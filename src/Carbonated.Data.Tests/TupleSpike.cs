﻿using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Carbonated.Data.Tests;

[TestFixture]
public class TupleSpike
{
    [Test]
    public void Test()
    {
        var record = new MockDataRecord(("id", 1), ("name", "John"));
        var dataReader = new MockDataReader(record);

        var b = typeof((int, int));
        var c = typeof((int, int, int));

        var reader = new EntityReader<(int, string)>(dataReader, new TupleMapper<(int, string)>());

        var inst = reader.First();

        Assert.Multiple(() =>
        {
            Assert.That(inst.Item1, Is.EqualTo(1));
            Assert.That(inst.Item2, Is.EqualTo("John"));
        });
    }
}

internal class TupleMapper<T> : Internals.Mapper<T>
{
    public TupleMapper()
    {
        if (EntityType != typeof(ValueTuple) && EntityType.BaseType == typeof(ValueTuple))
        {
            throw new Exception("You need a tuple!");
        }
    }

    protected internal override T CreateInstance(Record record)
    {
        var fields = EntityType.GetTypeInfo().DeclaredFields;
        int fieldCount = fields.Count();

        var entity = (object)((int)record[0], (string)record[1]);

        return (T)entity;
    }
}
