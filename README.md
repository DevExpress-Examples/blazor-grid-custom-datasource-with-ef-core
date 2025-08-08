<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1299285)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# Blazor Grid - Bind to a Custom Data Source using Entity Framework Core

This example implements a custom data source and binds it to the [DevExpress Blazor Grid](https://docs.devexpress.com/Blazor/403143/components/grid) component using [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/).

![Custom Data Source](image.png)

Our Blazor Grid component supports [various data binding scenarios](https://docs.devexpress.com/Blazor/403737/components/grid/bind-to-data). If none of standard data sources meet your requirements, you can implement your own data provider. Create a [GridCustomDataSource](https://docs.devexpress.com/Blazor/DevExpress.Blazor.GridCustomDataSource) descendant and assign it to the Grid [Data](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.Data) property. As a starting point, you can use the [OrdersDataSource](./CS/CustomDataSource/Services/OrdersDataSource.cs) class implementation and modify it based on your data/requirements.

## Specifics and Limitations

A custom data source imposes the following limitations on Grid features:

* [Custom sorting](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.CustomSort) is not supported.
* [Interval](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn.GroupInterval)/[custom](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn.GroupInterval#custom-grouping) grouping is not supported.
* [Search](https://docs.devexpress.com/Blazor/404142/components/grid/data-shaping/filter-data/search-box), [filter](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn.FilterMode#filter-data-by-display-text), [sorting](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn.SortMode#sort-data-by-display-text), and [grouping](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn.GroupInterval) by display text are not supported.
* Sorting, filtering, or grouping data during the _select_/_deselect all_ operation processing cancels the operation.
* [Custom summary calculation](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.CustomSummary) is not supported.
* [Unbound columns](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn#create-an-unbound-column) are not supported.
* To call the [SetFocusedDataItemAsync](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.SetFocusedDataItemAsync(System.Object)) method, you should specify the [KeyFieldName](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.KeyFieldName)/[KeyFieldNames](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.KeyFieldNames) property value.
* `AllPages` mode of the [Select All checkbox](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.KeyFieldNames) is not supported.
* [SelectAllAsync](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.SelectAllAsync(System.Boolean)) and [DeselectAllAsync](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.DeselectAllAsync) methods load all data to the grid and can reduce overall performance and increase memory consumption.
* The second call to the [SelectAllAsync](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.SelectAllAsync(System.Boolean))/[DeselectAllAsync](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.DeselectAllAsync) method cancels the operation initiated by the previously called method.

## Files to Review

- [OrdersDataSource.cs](./CS/CustomDataSource/Services/OrdersDataSource.cs)
- [Index.razor](./CS/CustomDataSource/Components/Pages/Index.razor)
- [Order.cs](./CS/CustomDataSource/Models/Order.cs)

## Documentation

- [Bind Blazor Grid to Data](https://docs.devexpress.com/Blazor/403737/components/grid/bind-to-data)

## More Examples

- [Blazor Grid - ExpandoObject Collection Support](https://github.com/DevExpress-Examples/blazor-editable-grid-with-expandoobject)
- [Blazor Grid – Binding to a DevExtreme Data Source with Entity Framework Core](https://github.com/DevExpress-Examples/blazor-dxgrid-bind-to-devextreme-data-source)

<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-grid-custom-datasource-with-ef-core&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-grid-custom-datasource-with-ef-core&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->



