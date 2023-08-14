using Carbonated.Data.Internals;
using Carbonated.Data.Tests.Types;
using NUnit.Framework;
using System.Collections.Generic;

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

        Assert.IsTrue(edr.Read());
        Assert.AreEqual("Foo", edr["Name"]);
        Assert.AreEqual("E1", edr[2]);

        Assert.IsTrue(edr.Read());
        Assert.AreEqual("Bar", edr[1]);
        Assert.AreEqual("E2", edr["Title"]);

        Assert.IsFalse(edr.Read());
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

        Assert.IsTrue(edr.Read());
        Assert.AreEqual(1, edr[0]);
        Assert.AreEqual(86, edr["agentno"]);

        Assert.IsTrue(edr.Read());
        Assert.AreEqual(2, edr[0]);
        Assert.AreEqual(99, edr["agentno"]);

        Assert.IsFalse(edr.Read());
    }

    //TODO: ignored fields are ignored
}
