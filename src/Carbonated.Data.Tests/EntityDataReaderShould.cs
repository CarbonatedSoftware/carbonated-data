using System.Collections.Generic;
using Carbonated.Data.Internals;
using Carbonated.Data.Tests.Types;
using NUnit.Framework;

namespace Carbonated.Data.Tests;

public class EntityDataReaderShould
{
    [Test]
    public void ReadAutoConfiguredEntity()
    {
        var propertyMapper = new MapperCollection().Configure<Entity>();

        var entities = new List<Entity>()
        {
            new(1, "Foo", "E1"),
            new(2, "Bar", "E2")
        };

        var edr = new EntityDataReader<Entity>(entities, propertyMapper);

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr["Name"], Is.EqualTo("Foo"));
            Assert.That(edr[2], Is.EqualTo("E1"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr[1], Is.EqualTo("Bar"));
            Assert.That(edr["Title"], Is.EqualTo("E2"));
        });

        Assert.That(edr.Read(), Is.False);
    }

    [Test]
    public void ReadEntityWithCustomMappingAndConverter()
    {
        var propertyMapper = new MapperCollection().Configure<EntityWithSemanticProperty>()
            .Map(x => x.AgentNumber, "agentno", x => new SemanticInt((int)x), x => ((SemanticInt)x).Value);

        var entities = new List<EntityWithSemanticProperty>()
        {
            new() { Id = 1, AgentNumber = new SemanticInt(86) },
            new() { Id = 2, AgentNumber = new SemanticInt(99) }
        };

       var edr = new EntityDataReader<EntityWithSemanticProperty>(entities, propertyMapper);

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr[0], Is.EqualTo(1));
            Assert.That(edr["agentno"], Is.EqualTo(86));
        });

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr[0], Is.EqualTo(2));
            Assert.That(edr["agentno"], Is.EqualTo(99));
        });

        Assert.That(edr.Read(), Is.False);
    }

    [Test]
    public void IgnoreFieldsMarkedAsIgnoreBoth()
    {
        var propertyMapper = new MapperCollection().Configure<Entity>()
            .Ignore(x => x.Name);

        var entities = new List<Entity>()
        {
            new(1, "Foo", "E1"),
            new(2, "Bar", "E2")
        };

        var edr = new EntityDataReader<Entity>(entities, propertyMapper);

        Assert.That(edr.FieldCount, Is.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr[0], Is.EqualTo(1));
            Assert.That(edr[1], Is.EqualTo("E1"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr["Id"], Is.EqualTo(2));
            Assert.That(edr["Title"], Is.EqualTo("E2"));
            Assert.That(() => edr["Name"], Throws.Exception);
        });

        Assert.That(edr.Read(), Is.False);
    }

    [Test]
    public void IgnoreFieldsMarkedAsIgnoreOnSave()
    {
        var propertyMapper = new MapperCollection().Configure<Entity>()
            .IgnoreOnSave(x => x.Name);

        var entities = new List<Entity>()
        {
            new(1, "Foo", "E1"),
            new(2, "Bar", "E2")
        };

        var edr = new EntityDataReader<Entity>(entities, propertyMapper);

        Assert.That(edr.FieldCount, Is.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr[0], Is.EqualTo(1));
            Assert.That(edr[1], Is.EqualTo("E1"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr["Id"], Is.EqualTo(2));
            Assert.That(edr["Title"], Is.EqualTo("E2"));
            Assert.That(() => edr["Name"], Throws.Exception);
        });

        Assert.That(edr.Read(), Is.False);
    }

    [Test]
    public void NotIgnoreFieldsMarkedAsIgnoreOnLoad()
    {
        var propertyMapper = new MapperCollection().Configure<Entity>()
            .IgnoreOnLoad(x => x.Name);

        var entities = new List<Entity>()
        {
            new(1, "Foo", "E1"),
            new(2, "Bar", "E2")
        };

        var edr = new EntityDataReader<Entity>(entities, propertyMapper);

        Assert.That(edr.FieldCount, Is.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr[0], Is.EqualTo(1));
            Assert.That(edr[1], Is.EqualTo("Foo"));
            Assert.That(edr[2], Is.EqualTo("E1"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(edr.Read(), Is.True);
            Assert.That(edr["Id"], Is.EqualTo(2));
            Assert.That(edr["Name"], Is.EqualTo("Bar"));
            Assert.That(edr["Title"], Is.EqualTo("E2"));
        });

        Assert.That(edr.Read(), Is.False);
    }
}
