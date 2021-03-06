# Design Notes

## Query Table notes

The `QueryTable()` method on `DbConnector` has a number of special case considerations. Originally, it worked by calling the data adapter's `Fill()` on a `DataTable`. This was updated to fill a `DataSet` instead so that the `EnforceConstraints` setting could be set to `false` if needed. (This came about when we ran across a scenario where data constraints had been overridden in a table that we need to process, and since it was client data, we had no choice but to accept the violation.)

A side effect of this update is that the DataTable's name was no longer populated automatically based on the query. When `FillSchema()` is called for a DataSet, it instead sets the table name to 'Table'. This happens under both .NET Framework and .NET Core.

As it turns out, using the table fill under Core would be broken anyway; instead of filling a table name, the name is left empty.

As a result, we need to populate the table name correctly so that round-trip saves work without requiring the consuming code to set a table name.

- Method 1: update the method call to take in a Query object of some sort to ensure we get an accurate table name
- Method 2: update the method call to expect a single table name (this breaks with the rest of the interface, where a single word is assumed to be a proc name)
- Method 3: attempt to parse the select statement looking for "from {TABLE}" to extract the table name
- Method 4: do nothing and expect the user to set the name should they wish to round-trip
- Alternatives: research autogenerated save adapaters to see if we can avoid needing to build them ourselves
  - https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/generating-commands-with-commandbuilders
  - https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/updating-data-sources-with-dataadapters
