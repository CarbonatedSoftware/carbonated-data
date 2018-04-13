# Carbonated Data
Data access library.

## Components

Connector - Manages commands and connections, allows querying of database.

Mapper - Maps data records to entities via property or type mapping. Subtypes are PropertyMapper and TypeMapper.

Record - Proxy for the record that adds fuzzy name matching semantics for reading from a record.

Reader - Wrapper around an IDataReader that adds IEnumerable semantics and returns entity objects.

DataContext - Main data access interface. A context can be opened from a connector so that everything under it will share a connection.

DbFactory - Interface that DB platform specific libraries will implement to create the appropriately typed DbObjects

ParamGenerator - Generates save parameters for an entity.

ParamGeneratorMapper - Mapping for the param generator.
