using System;
using Carbonated.Data.Internals;
using Carbonated.Data.Tests.Types;
using NUnit.Framework;

namespace Carbonated.Data.Tests;

[TestFixture]
public class MapperCollectionShould
{
    private MapperCollection mc;

    [SetUp]
    public void SetUp()
    {
        mc = new MapperCollection();
    }

    [Test]
    public void AddTypeMapperWhenCreateFuncIsConfigured()
    {
        mc.Configure<Entity>(record => new Entity());

        Assert.That(mc.Get<Entity>(), Is.InstanceOf<TypeMapper<Entity>>());
    }

    [Test]
    public void AddPropertyMapperAndReturnItWhenConfigured()
    {
        var mapper = mc.Configure<Entity>();

        Assert.That(mapper, Is.InstanceOf<PropertyMapper<Entity>>());
    }

    [Test]
    public void AddPropertyMapperWithDefaultConditionAndReturnItWhenConfigured()
    {
        var mapper = mc.Configure<Entity>(PopulationCondition.Required);

        Assert.That(mapper, Is.InstanceOf<PropertyMapper<Entity>>());
    }

    [Test]
    public void KnowWhatMappersHaveBeenConfigured()
    {
        mc.Configure<Entity>();

        Assert.That(mc.HasMapper<Entity>(), Is.True);
    }

    [Test]
    public void ThrowOnConfigureIfTypeIsAlreadyRegistered()
    {
        mc.Configure<Entity>();

        Assert.That(() => mc.Configure<Entity>(), Throws.TypeOf<MappingException>());
    }

    [Test]
    public void AutoGenerateAndAddPropertyMapperDuringGetIfNotYetRegistered()
    {
        Assert.That(mc.HasMapper<Entity>(), Is.False);

        var mapper = mc.Get<Entity>();

        Assert.Multiple(() =>
        {
            Assert.That(mc.HasMapper<Entity>(), Is.True);
            Assert.That(mapper, Is.InstanceOf<PropertyMapper<Entity>>());
        });
    }

    [Test]
    public void HaveFrameworkTypeMappersBuiltIn()
    {
        Assert.Multiple(() =>
        {
            Assert.That(mc.HasMapper<bool>(), Is.True);
            Assert.That(mc.HasMapper<byte>(), Is.True);
            Assert.That(mc.HasMapper<short>(), Is.True);
            Assert.That(mc.HasMapper<int>(), Is.True);
            Assert.That(mc.HasMapper<long>(), Is.True);
            Assert.That(mc.HasMapper<float>(), Is.True);
            Assert.That(mc.HasMapper<double>(), Is.True);
            Assert.That(mc.HasMapper<decimal>(), Is.True);
            Assert.That(mc.HasMapper<DateTime>(), Is.True);
            Assert.That(mc.HasMapper<DateOnly>(), Is.True);
            Assert.That(mc.HasMapper<TimeOnly>(), Is.True);
            Assert.That(mc.HasMapper<Guid>(), Is.True);
            Assert.That(mc.HasMapper<char>(), Is.True);
            Assert.That(mc.HasMapper<string>(), Is.True);
            Assert.That(mc.HasMapper<byte[]>(), Is.True);

            Assert.That(mc.HasMapper<bool?>(), Is.True);
            Assert.That(mc.HasMapper<byte?>(), Is.True);
            Assert.That(mc.HasMapper<short?>(), Is.True);
            Assert.That(mc.HasMapper<int?>(), Is.True);
            Assert.That(mc.HasMapper<long?>(), Is.True);
            Assert.That(mc.HasMapper<float?>(), Is.True);
            Assert.That(mc.HasMapper<double?>(), Is.True);
            Assert.That(mc.HasMapper<decimal?>(), Is.True);
            Assert.That(mc.HasMapper<DateTime?>(), Is.True);
            Assert.That(mc.HasMapper<DateOnly?>(), Is.True);
            Assert.That(mc.HasMapper<TimeOnly?>(), Is.True);
            Assert.That(mc.HasMapper<Guid?>(), Is.True);
            Assert.That(mc.HasMapper<char?>(), Is.True);
        });
    }
}
