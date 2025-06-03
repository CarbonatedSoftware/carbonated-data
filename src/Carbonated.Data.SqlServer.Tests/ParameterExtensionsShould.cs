using System;
using System.Data;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Carbonated.Data.SqlServer.Tests;

[TestFixture]
public class ParameterExtensionsShould
{
    [Test]
    public void CreateCharParamWithValue()
    {
        string value = "fixed";
        var param = (SqlParameter)value.AsCharParam(5);

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.AnsiStringFixedLength));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Char));
            Assert.That(param.Value, Is.EqualTo(value));
            Assert.That(param.Size, Is.EqualTo(5));
        });
    }

    [Test]
    public void CreateCharParamWithDbNullAsValue()
    {
        string value = null;
        var param = (SqlParameter)value.AsCharParam(5);

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.AnsiStringFixedLength));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Char));
            Assert.That(param.Value, Is.EqualTo(DBNull.Value));
            Assert.That(param.Size, Is.EqualTo(5));
        });
    }

    [Test]
    public void CreateNCharParamWithValue()
    {
        string value = "fixed";
        var param = (SqlParameter)value.AsNCharParam(5);

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.StringFixedLength));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.NChar));
            Assert.That(param.Value, Is.EqualTo(value));
            Assert.That(param.Size, Is.EqualTo(5));
        });
    }

    [Test]
    public void CreateNCharParamWithDbNullAsValue()
    {
        string value = null;
        var param = (SqlParameter)value.AsNCharParam(5);

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.StringFixedLength));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.NChar));
            Assert.That(param.Value, Is.EqualTo(DBNull.Value));
            Assert.That(param.Size, Is.EqualTo(5));
        });
    }

    [Test]
    public void CreateVarCharParamWithValue()
    {
        string value = "var";
        var param = (SqlParameter)value.AsVarCharParam(50);

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.AnsiString));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.VarChar));
            Assert.That(param.Value, Is.EqualTo(value));
            Assert.That(param.Size, Is.EqualTo(50));
        });
    }

    [Test]
    public void CreateVarCharParamWithDbNullAsValue()
    {
        string value = null;
        var param = (SqlParameter)value.AsVarCharParam(50);

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.AnsiString));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.VarChar));
            Assert.That(param.Value, Is.EqualTo(DBNull.Value));
            Assert.That(param.Size, Is.EqualTo(50));
        });
    }

    [Test]
    public void CreateNVarCharParamWithValue()
    {
        string value = "nvarchar";
        var param = (SqlParameter)value.AsNVarCharParam(50);

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.String));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.NVarChar));
            Assert.That(param.Value, Is.EqualTo(value));
            Assert.That(param.Size, Is.EqualTo(50));
        });
    }

    [Test]
    public void CreateNVarCharParamWithDbNullAsValue()
    {
        string value = null;
        var param = (SqlParameter)value.AsNVarCharParam(50);

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.String));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.NVarChar));
            Assert.That(param.Value, Is.EqualTo(DBNull.Value));
            Assert.That(param.Size, Is.EqualTo(50));
        });
    }

    [Test]
    public void FromDateTime_CreateDateParamWithValue()
    {
        var date = new DateTime(2024, 1, 1);
        var param = (SqlParameter)date.AsDateParam();

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.Date));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Date));
            Assert.That(param.Value, Is.EqualTo(date));
        });
    }

    [Test]
    public void FromNullableDateTime_CreateDateParamWithValue()
    {
        DateTime? date = new DateTime(2024, 1, 2);
        var param = (SqlParameter)date.AsDateParam();

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.Date));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Date));
            Assert.That(param.Value, Is.EqualTo(date.Value));
        });
    }

    [Test]
    public void FromNullableDateTime_CreateDateParameterWithDbNullAsValue()
    {
        DateTime? date = null;
        var param = (SqlParameter)date.AsDateParam();

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.Date));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Date));
            Assert.That(param.Value, Is.EqualTo(DBNull.Value));
        });
    }

    [Test]
    public void FromDateTime_CreateDateTime2ParamWithValue()
    {
        var date = new DateTime(2024, 1, 3, 12, 30, 0);
        var param = (SqlParameter)date.AsDateTime2Param();

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.DateTime2));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.DateTime2));
            Assert.That(param.Value, Is.EqualTo(date));
        });
    }

    [Test]
    public void FromNullableDateTime_CreateDateTime2ParamWithValue()
    {
        DateTime? date = new DateTime(2024, 1, 4, 15, 45, 0);
        var param = (SqlParameter)date.AsDateTime2Param();

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.DateTime2));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.DateTime2));
            Assert.That(param.Value, Is.EqualTo(date.Value));
        });
    }

    [Test]
    public void FromNullableDateTime_CreateDateTime2ParamWithDbNullAsValue()
    {
        DateTime? date = null;
        var param = (SqlParameter)date.AsDateTime2Param();

        Assert.Multiple(() =>
        {
            Assert.That(param.DbType, Is.EqualTo(DbType.DateTime2));
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.DateTime2));
            Assert.That(param.Value, Is.EqualTo(DBNull.Value));
        });
    }

    [Test]
    public void FromObject_CreateTableValueParam()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Rows.Add(1);

        var param = (SqlParameter)((object)table).AsTableValueParam("dbo.MyType");

        Assert.Multiple(() =>
        {
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Structured));
            Assert.That(param.TypeName, Is.EqualTo("dbo.MyType"));
            Assert.That(param.Value, Is.EqualTo(table));
        });
    }

    [Test]
    public void FromObject_CreateTableValueParamWithDbNull()
    {
        object value = null;
        var param = (SqlParameter)value.AsTableValueParam("dbo.MyType");

        Assert.Multiple(() =>
        {
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Structured));
            Assert.That(param.TypeName, Is.EqualTo("dbo.MyType"));
            Assert.That(param.Value, Is.EqualTo(DBNull.Value));
        });
    }

    [Test]
    public void FromDataTable_CreateTableValueParam()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Rows.Add(2);

        var param = (SqlParameter)table.AsTableValueParam("dbo.MyType");

        Assert.Multiple(() =>
        {
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Structured));
            Assert.That(param.TypeName, Is.EqualTo("dbo.MyType"));
            Assert.That(param.Value, Is.EqualTo(table));
        });
    }

    [Test]
    public void FromDataTable_CreateTableValueParamWithDbNull()
    {
        DataTable table = null;
        var param = (SqlParameter)table.AsTableValueParam("dbo.MyType");

        Assert.Multiple(() =>
        {
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Structured));
            Assert.That(param.TypeName, Is.EqualTo("dbo.MyType"));
            Assert.That(param.Value, Is.EqualTo(DBNull.Value));
        });
    }

    [Test]
    public void FromDataReader_CreateTableValueParam()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Rows.Add(3);

        using var reader = table.CreateDataReader();
        var param = (SqlParameter)reader.AsTableValueParam("dbo.MyType");

        Assert.Multiple(() =>
        {
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Structured));
            Assert.That(param.TypeName, Is.EqualTo("dbo.MyType"));
            Assert.That(param.Value, Is.EqualTo(reader));
        });
    }

    [Test]
    public void FromDataReader_CreateTableValueParamWithDbNull()
    {
        IDataReader reader = null;
        var param = (SqlParameter)reader.AsTableValueParam("dbo.MyType");

        Assert.Multiple(() =>
        {
            Assert.That(param.SqlDbType, Is.EqualTo(SqlDbType.Structured));
            Assert.That(param.TypeName, Is.EqualTo("dbo.MyType"));
            Assert.That(param.Value, Is.EqualTo(DBNull.Value));
        });
    }
}
