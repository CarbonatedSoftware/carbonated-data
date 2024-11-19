# About

The Carbonated Data SQL library extends the base Carbonated.Data package with SQL Server connectors and object factories.


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
