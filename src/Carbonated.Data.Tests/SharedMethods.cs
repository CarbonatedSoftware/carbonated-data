using System;
using System.Collections.Generic;
using Carbonated.Data.Internals;

namespace Carbonated.Data.Tests;

public static class SharedMethods
{
    public static Record Record(params (string field, object value)[] fields)
    {
        var record = new MockDataRecord(fields);
        var reader = new MockDataReader(record);
        return new Data.Record(reader);
    }

    public static string[] Strings(params string[] strings) => strings;

    public static PropertyMapper<T> PropMapper<T>() => new(null);

    public static PropertyMapper<T> PropMapper<T>(IDictionary<Type, ValueConverter> converters) 
        => new(converters);

    public static PropertyMapper<T> PropMapper<T>(PopulationCondition condition) 
        => new(null, condition);
}
