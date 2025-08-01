using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using CustomDataSource.Models;
using System.Collections;
using System.Linq.Expressions;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CustomDataSource.Services;

public class OrdersDataSource : GridCustomDataSource {
    protected override Type DataItemType => typeof(Order);
    private readonly NorthwindContext _context;

    public OrdersDataSource(IDbContextFactory<NorthwindContext> contextFactory) {
        _context = contextFactory.CreateDbContext();
    } 

    public override async Task<int> GetItemCountAsync(GridCustomDataSourceCountOptions options, CancellationToken cancellationToken) {
        return await ApplyFiltering(options.FilterCriteria, _context.Orders)
            .CountAsync(cancellationToken);
    }

    public override async Task<IList> GetItemsAsync(GridCustomDataSourceItemsOptions options, CancellationToken cancellationToken) {
        var filteredQuery = ApplyFiltering(options.FilterCriteria, _context.Orders);

        if (options.Count >= 0) {
            return await ApplySorting(options, filteredQuery)
                .Skip(options.StartIndex)
                .Take(options.Count)
                .ToListAsync(cancellationToken);
        }

        return await ApplySorting(options, filteredQuery).ToListAsync(cancellationToken);
    }

    public override async Task<object[]> GetUniqueValuesAsync(GridCustomDataSourceUniqueValuesOptions options, CancellationToken cancellationToken) {
        var filteredQuery = ApplyFiltering(options.FilterCriteria, _context.Orders);
        var lambda = GetTypedLambda(options.FieldName);
        return await filteredQuery
            .Select(lambda)
            .Distinct()
            .OrderBy(x => x)
            .ToArrayAsync(cancellationToken);
    }

    #region Summary methods

    public override async Task<IList<GridCustomDataSourceGroupInfo>> GetGroupInfoAsync(GridCustomDataSourceGroupingOptions options,
        CancellationToken cancellationToken) {
        var filteredQuery = ApplyFiltering(options.FilterCriteria, _context.Orders);

        var summaryQuery = GetGroupByAndSelectQuery(options.SummaryInfo, filteredQuery, false, options.FieldName);
        var resultList = await summaryQuery.ToListAsync(cancellationToken);

        var groupInfoList = new List<GridCustomDataSourceGroupInfo>();
        foreach (var result in resultList) {
            var groupInfo = new GridCustomDataSourceGroupInfo() { Value = result["Value"], DataItemCount = (int)result["DataItemCount"] };

            var summaryList = new List<object>();
            foreach (var kvp in result) {
                if (kvp.Key.StartsWith("Summary_")) {
                    summaryList.Add(kvp.Value);
                }
            }

            groupInfo.SummaryValues = summaryList;
            groupInfoList.Add(groupInfo);
        }

        return options.DescendingSortOrder
            ? groupInfoList
                .OrderByDescending(g => g.Value)
                .ToList()
            : groupInfoList
                .OrderBy(g => g.Value)
                .ToList();
    }

    public override async Task<IList> GetTotalSummaryAsync(GridCustomDataSourceTotalSummaryOptions options, CancellationToken cancellationToken) {
        var filteredQuery = ApplyFiltering(options.FilterCriteria, _context.Orders);

        var summaryQuery = GetGroupByAndSelectQuery(options.SummaryInfo, filteredQuery, true);
        var result = await summaryQuery.FirstOrDefaultAsync(cancellationToken);
        return result != null
            ? result
                .Select(d => d.Value)
                .ToList()
            : null;
    }

    #endregion

    #region Helper methods

    private static IQueryable<Order> ApplyFiltering(CriteriaOperator criteria, IQueryable<Order> queryableSource) {
        return !criteria.ReferenceEqualsNull()
            ? (IQueryable<Order>)queryableSource.AppendWhere(
                new CriteriaToEFExpressionConverter(queryableSource.Provider.GetType()), criteria)
            : queryableSource;
    }

    private static IQueryable<Order> ApplySorting(GridCustomDataSourceItemsOptions options, IQueryable<Order> queryableSource) {
        if (options.SortInfo != null) {
            foreach (var sortInfo in options.SortInfo) {
                queryableSource = SortByField(sortInfo.FieldName, sortInfo.DescendingSortOrder, queryableSource);
            }
        }

        return queryableSource;
    }

    private static IQueryable<Order> SortByField(string fieldName, bool descendingOrder, IQueryable<Order> queryableSource) {
        var lambda = GetTypedLambda(fieldName);
        if (queryableSource.Expression.Type != typeof(IOrderedQueryable<Order>)) {
            return !descendingOrder
                ? queryableSource.OrderBy(lambda)
                : queryableSource.OrderByDescending(lambda);
        }

        return !descendingOrder
            ? ((IOrderedQueryable<Order>)queryableSource).ThenBy(lambda)
            : ((IOrderedQueryable<Order>)queryableSource).ThenByDescending(lambda);
    }

    private static Expression<Func<Order, object>> GetTypedLambda(string fieldName) {
        var parameter = Expression.Parameter(typeof(Order));
        var property = Expression.Property(parameter, fieldName);
        var lambda = Expression.Lambda<Func<Order, object>>(Expression.Convert(property, typeof(object)), parameter);
        return lambda;
    }

    private static IQueryable<Dictionary<string, object>> GetGroupByAndSelectQuery(IReadOnlyList<GridCustomDataSourceSummaryInfo> summaryInfoList,
        IQueryable<Order> queryableSource, bool totalSummary, string groupedFieldName = "") {
        IQueryable groupedQuery;
        Type groupedPropertyType;
        if (totalSummary) {
            groupedPropertyType = typeof(int);
            groupedQuery = queryableSource.GroupBy(x => 1);
        } else {
            var parameter = Expression.Parameter(typeof(Order));
            var property = Expression.Property(parameter, groupedFieldName);
            groupedPropertyType = property.Type;
            var groupLambda = Expression.Lambda(property, parameter);

            var groupByCall = Expression.Call(typeof(Queryable), "GroupBy", new[] { typeof(Order), groupedPropertyType },
                queryableSource.Expression, groupLambda);
            groupedQuery = queryableSource.Provider.CreateQuery(groupByCall);
        }

        var groupType = typeof(IGrouping<,>).MakeGenericType(groupedPropertyType, typeof(Order));
        var group = Expression.Parameter(groupType);

        // defining a dictionary that will hold group info
        var dictItems = new List<ElementInit>();
        var addMethod = typeof(Dictionary<string, object>).GetMethod("Add");
        if (!totalSummary) {
            dictItems.AddRange(new List<ElementInit> {
                Expression.ElementInit(addMethod, Expression.Constant("Value"),
                    Expression.Convert(Expression.Property(group, "Key"), typeof(object))
                ),
                Expression.ElementInit(addMethod, Expression.Constant("DataItemCount"),
                    Expression.Convert(Expression.Call(typeof(Enumerable), "Count",
                        new[] { typeof(Order) }, group), typeof(object))
                )
            });
        }

        for (int i = 0; i < summaryInfoList.Count; i++) {
            var summaryInfo = summaryInfoList[i];

            var groupSummaryCall = GetGroupSummaryCall(summaryInfo, group);
            dictItems.Add(Expression.ElementInit(addMethod, Expression.Constant("Summary_" + i),
                Expression.Convert(groupSummaryCall, typeof(object))));
        }

        var dictInit = Expression.ListInit(Expression.New(typeof(Dictionary<string, object>)), dictItems);

        // creating the select query
        Type selectFuncType = typeof(Func<,>).MakeGenericType(groupType, typeof(Dictionary<string, object>));
        var selectLambda = Expression.Lambda(selectFuncType, dictInit, group);
        var selectCall = Expression.Call(typeof(Queryable), "Select",
            new[] { groupedQuery.ElementType, typeof(Dictionary<string, object>) },
            groupedQuery.Expression, selectLambda);

        return groupedQuery.Provider.CreateQuery<Dictionary<string, object>>(selectCall);
    }

    private static Expression GetGroupSummaryCall(GridCustomDataSourceSummaryInfo summaryInfo,
        Expression group) {
        var parameter = Expression.Parameter(typeof(Order));
        var property = Expression.Property(parameter, summaryInfo.FieldName);
        var summaryLambda = Expression.Lambda(property, parameter);

        switch (summaryInfo.SummaryType) {
            case GridSummaryItemType.Avg:
                return Expression.Call(typeof(Enumerable), "Average", new[] { typeof(Order) },
                    group, summaryLambda);
            case GridSummaryItemType.Count:
                return Expression.Call(typeof(Enumerable), "Count", new[] { typeof(Order) },
                    group);
            case GridSummaryItemType.Max:
                return Expression.Call(typeof(Enumerable), "Max", new[] { typeof(Order), property.Type },
                    group, summaryLambda);
            case GridSummaryItemType.Min:
                return Expression.Call(typeof(Enumerable), "Min", new[] { typeof(Order), property.Type },
                    group, summaryLambda);
            case GridSummaryItemType.Sum:
                return Expression.Call(typeof(Enumerable), "Sum", new[] { typeof(Order) },
                    group, summaryLambda);
            default:
                throw new NotSupportedException(summaryInfo.SummaryType.ToString());
        }
    }

    #endregion
}
