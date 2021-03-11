# DbConnector

The `DbConnector` is a data access connector that automatically manages commands and connections for queries. It is the center of the Carbonated Data library, and the starting point for most code interaction.

The connector depends on a `DbObjectFactory` to create the needed underlying .NET data access objects. The factory is specific to the SQL database that is being connected to. SQL Server connectivity is provided by another library in this project, for example.

## Usage Examples

...


## Query Methods

The *Query* methods of the connector are used for querying the database in various ways and loading the results into entities, readers, etc.

All of the *Query* methods of the Connector follow the same pattern in their inputs, accepting a base SQL query and parameters. The details of the parameters and usage shown here apply to all of them. They differ only in the types that are returned from them.

### *QueryMethod*(sql, parameters)

```c#
<ret> QueryMethod(string sql, params (string name, object value)[] parameters);
<ret> QueryMethod(string sql, IEnumerable<DbParameter> parameters = null);
```

#### Parameters

**`sql`**: An ad hoc script, or the name of a stored procedure to execute. If the argument value is a single word, with no spaces, it will be assumed to be the name of a stored procedure and execute that way. If there are spaces, it will be assumed to be an ad hoc text command.

**`parameters`**: Optionally, the parameters to use if the query or stored procedure needs them. This takes the form of either a `params` array of name/value tuples, or an `IEnumerable` collection of `DbParameter`. If the ad hoc query does include parameters, the query text should follow standard parameterized SQL syntax: `"select * from table where field = @paramname"`

#### Returns

The SQL query or procedure will be executed with the parameters provided and the results will be returned.

--------

### NonQuery(sql, parameters)

```c#
int NonQuery(string sql, params (string name, object value)[] parameters);
int NonQuery(string sql, IEnumerable<DbParameter> parameters = null);
```

Executes a SQL query with no result; the return value is the number of rows affected by the query.

#### Examples

```c#
int affectedCount = connector.NonQuery("update sample set flag = 1 where id = @id", ("@id", 42));
```
--------

### Query\<TEntity>(sql, parameters)

```c#
IEnumerable<TEntity> Query<TEntity>(string sql, params (string name, object value)[] parameters);
IEnumerable<TEntity> Query<TEntity>(string sql, IEnumerable<DbParameter> parameters = null);
```

Executes a SQL query and returns the result as an enumerable collection of entities of type `TEntity`. The entities will be hydrated from the underlying data based on automatic mappings, combined with any entity mapping configurations that have been set via the connector's `Mappers`.

#### Examples

```c#
// A sample entity type
class Color {
  string Name { get; set; }
  string HexValue { get; set; }
}

// Ad hoc query with no parameters
var colors = connector.Query<Color>("select name, hexvalue from colors");

// Ad hoc query with parameters
var colors = connector.Query<Color>("select name, hexvalue from colors where type = @colorType", ("@colorType", "primary"));

// Stored procedure with no parameters
var colors = connector.Query<Color>("GetPrimaryColors");

// Stored procedure with parameters
var colors = connector.Query<Color>("GetColorsByType", ("@colorType", "secondary"));
```

--------

### QueryReader\<TEntity>(sql, parameters)

Executes a query and returns an `EntityReader<TEntity>` that can be used to read the results as `TEntity` in sequence.

...

--------

### QueryReader(sql, parameters)

Executes a query and returns a `DbDataReader` of the results. The results will not be automatically converted to an entity as they are read.

...

--------

### QueryScalar\<TEntity>(sql, parameters)

Executes a query and returns a single result of type `TEntity`.

...

--------

### QueryScalar(sql, parameters)

Executes a query and returns a single result as an `object`.

...

--------

### QueryTable(sql, parameters)

```c#
DataTable QueryTable(string sql, params (string name, object value)[] parameters);
DataTable QueryTable(string sql, IEnumerable<DbParameter> parameters);
```

Executes a SQL query and returns the result in a `DataTable`. The query can be either a stored procedure name or an ad hoc `SELECT` script.

If you want to round-trip the table using the SaveTable() method, you will need to ensure that the Table Name is set appropriately in the cases where it could not automatically be determined by the script.

**UPCOMING FEATURE:** It attempts to load the table schema and properly set the table name when provided with a script in the form of "SELECT {FIELDS} FROM {TABLE}...". If the query uses a Stored Procedure, has JOINs, or is in a form that the code is unable to correctly parse, the table name will be empty.

#### Examples

```c#
var table = dbConnector.QueryTable("select * from sample where region = @regionCode", ("@regionCode", 86));
```

--------

## Other Methods

### OpenContext()

```c#
DbContext OpenContext();
```

Opens a connection and returns a data context that will operate against that signle connection until it is disposed. The context will use the same connection string and timeout settings as the parent connector that it is created from.

This allows multiple queries to be run using the same connection without closing it. When you are done with the context, you should dispose of it by calling `Dispose()`.

----

### SaveTable(table)

```c#
int SaveTable(DataTable table);
```

Saves the data from the table to the database. In order for this to work, the table name must be set, and the table must have a primary key. The primary key limitation is a requirement of the underlying .NET library code, and cannot be avoided. If you need to save table data with no primary key, you will need to create your own scripts to save the data.

`QueryTable()` attempts to give you a table that can be easily round-tripped back through `SaveTable()`, but there are cases where it may not be able to load the schema information or table name correctly. Please test your code to avoid surprises.
