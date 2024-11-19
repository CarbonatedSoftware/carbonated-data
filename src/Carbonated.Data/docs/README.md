# About

Carbonated Data is a data access library built on top of the .NET data access system. Its primary purpose is to allow quick and easy loading of tabular data from a SQL database directly into entities, eliminating the need to write table-to-entity conversion code.

It is not an ORM, and has no interest in controlling the structure of the database. It is focused on making querying and loading of relational data into entities as smooth and painless as possible.


# How to Use

```c#
// Examples setup

class City {
  public int Id { get; set; }
  public string Name { get; set; }
  public string State { get; set; }
  public int Population { get; set; }
}

string connString = "<load from cfg>";
```

```c#
// Create a SQL Server DbConnector from the Carbonated.Data.SqlServer pacakge and execute a simple query

var connector = new SqlServerDbConnector(connString);
var cities = connector.Query<City>("select * from cities");
```

```c#
// Create a DbConnector, passing in a SQL Server ObjectFactory, to execute a stored procedure with parameters

var connector = new DbConnector(new SqlServerObjectFactory(), connString);
var cities = connector.Query<City>("GetCitiesByState", ("@State", "TX"));
```
