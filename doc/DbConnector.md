# DbConnector

The `DbConnector` is a data access connector that automatically manages commands and connections for queries. It is the center of the Carbonated Data library, and the starting point for most code interaction.

The connector depends on a `DbObjectFactory` to create the needed underlying .NET data access objects. The factory is specific to the SQL database that is being connected to. SQL Server connectivity is provided by another library in this project, for example.

## Usage Examples

...


## Methods

These are the public methods of the connector, used primarily for querying the database and loading the results into entities.


### QueryTable(sql, parameters)

```c#
DataTable QueryTable(string sql, params (string name, object value)[] parameters)
DataTable QueryTable(string sql, IEnumerable<DbParameter> parameters)
```

Executes a SQL query and returns the result in a `DataTable`. The query can be either a stored procedure name or an ad-hoc `SELECT` script.

If the user wants to round-trip the table using the SaveTable() method, they will need to ensure that the Table Name is set appropriately in the cases where it could not automatically be determined by the script.

**UPCOMING FEATURE:** It attempts to load the table schema and properly set the table name when provided with a script in the form of "SELECT {FIELDS} FROM {TABLE}...". If the query uses a Stored Procedure, has JOINs, or is in a form that the code is unable to correctly parse, the table name will be empty.

#### Parameters

Parameter    | Description
-------------|--------------------------------
`sql`        | An ad hoc script or the name of a stored procedure to execute.
`parameters` | Either an array of name/value tuples, or an enumerable collection of `DbParameter`, holding the parameter names and values for any SQL parameters that occur within the script.

Parameters are optional, and are only needed if the SQL query includes parameter references. If it does include parameters, the query text should follow standard SQL `@paramname` syntax.

#### Examples

```c#
// Ad hoc query with no parameters
dbConnector.QueryTable("select * from sample");

// Ad hoc query with parameters
dbConnector.QueryTable("select * from sample where id = @id", ("@id", 42));

// Stored procedure with no parameters
dbConnector.QueryTable("GetSampleData");

// Stored procedure with parameters
dbConnector.QueryTable("GetSampleData", ("@id", 42));
```


### SaveTable()

This will save the provided table to the database. In order for this to work, the table name must be set, and the table must have a primary key. The primary key limitation is a requirement of the underlying .NET library code, and cannot be avoided. If you need to save table data with no primary key, you will need to create your own scripts to save the data.

QueryTable() attempts to give you a table that can be easily round-tripped back through SaveTable(), but there are cases where it may not be able to load the schema information or table name correctly. Please test your code to avoid surprises.
