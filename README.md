# Carbonated Data

Carbonated Data is a data access library built on top of the .NET data access system. Its primary purpose is to allow quick and easy loading of tabular data from a SQL database directly into entities, eliminating the need to write table-to-entity conversion code.

It is **not** an ORM, and has no interest in controlling the structure of the database, nor in bi-directional mapping of entities to relational data. It is focused on making querying and loading of relational data into entities as smooth and painless as possible.

## Components

[DbConnector](doc/DbConnector.md) - Manages commands and connections, allows querying of database.

Mapper - Maps data records to entities via property or type mapping. Subtypes are PropertyMapper and TypeMapper.

Record - Proxy for the record that adds fuzzy name matching semantics for reading from a record.

EntityReader - Wrapper around an IDataReader that adds IEnumerable semantics and returns entity objects.

DbContext - Main data access interface. A context can be opened from a connector so that everything under it will share a connection.

DbObjectFactory - Interface that DB platform specific libraries will implement to create the appropriately typed DbObjects

EntityDataReader - Allows an IEnumerable collection of entities to be read using the IDataReader interface. Used for SqlBulkCopy saving of large data sets.

## History and Design Philosophy

This is not the first data access library that I have created, but it's the first one that I'm actually satisfied serves a useful purpose without falling into architectural mistakes. While designing and developing this, I tried very hard to ensure that it was useful, but would not introduce rippling dependcies throughout the client codebase.

One the main goals of this library was to avoid the need to include it as dependency in your business logic layer. As a result, any special handling or customization is controlled via configuration on the DB Connector, not via base classes or attributes on the properties of your entities. I've tried to make the syntax for this as fluid and simple as possible, but it is a departure from the choices that most C#/.NET data access libraries make.
