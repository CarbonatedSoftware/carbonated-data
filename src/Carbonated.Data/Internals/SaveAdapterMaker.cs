﻿using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Carbonated.Data.Internals;

internal class SaveAdapterMaker
{
    private readonly DbObjectFactory dbFactory;
    private readonly int commandTimeout;

    internal SaveAdapterMaker(DbObjectFactory dbFactory, int commandTimeout)
    {
        this.dbFactory = dbFactory;
        this.commandTimeout = commandTimeout;
    }

    internal DbDataAdapter MakeAdapter(DataTable table, DbConnection connection, DbTransaction transaction)
    {
        var adapter = dbFactory.CreateDataAdapter();
        adapter.InsertCommand = MakeInsertCommand(table, connection, transaction);
        adapter.UpdateCommand = MakeUpdateCommand(table, connection, transaction);
        adapter.DeleteCommand = MakeDeleteCommand(table, connection, transaction);
        return adapter;
    }

    private DbCommand MakeInsertCommand(DataTable table, DbConnection connection, DbTransaction transaction)
    {
        var parameters = new List<DbParameter>();
        var insertColumns = new List<string>();
        var insertValues = new List<string>();
        foreach (DataColumn col in table.Columns)
        {
            if (!col.AutoIncrement)
            {
                string colName = col.ColumnName;
                insertColumns.Add(dbFactory.DelimitIdentifier(colName));
                insertValues.Add($"@{colName}");
                parameters.Add(CreateMappingParameter(colName, colName));
            }
        }

        string sql = $"insert into {table.TableName} ({string.Join(", ", insertColumns)}) values ({string.Join(", ", insertValues)})";
        return PrepareCommand(sql, parameters, connection, transaction);
    }

    private DbCommand MakeUpdateCommand(DataTable table, DbConnection connection, DbTransaction transaction)
    {
        var parameters = new List<DbParameter>();
        var setColumns = new List<string>();
        var keyColumns = new List<string>();
        foreach (DataColumn col in table.Columns)
        {
            string colName = col.ColumnName;
            if (table.PrimaryKey.Contains(col))
            {
                keyColumns.Add($"{dbFactory.DelimitIdentifier(colName)} = @{colName}");
            }
            else
            {
                setColumns.Add($"{dbFactory.DelimitIdentifier(colName)} = @{colName}");
            }
            parameters.Add(CreateMappingParameter(colName, colName));
        }

        string sql = $"update {table.TableName} set {string.Join(", ", setColumns)} where {string.Join(" and ", keyColumns)}";
        return PrepareCommand(sql, parameters, connection, transaction);
    }

    private DbCommand MakeDeleteCommand(DataTable table, DbConnection connection, DbTransaction transaction)
    {
        var parameters = new List<DbParameter>();
        var keyColumns = new List<string>();
        foreach (var col in table.PrimaryKey)
        {
            string colName = col.ColumnName;
            keyColumns.Add($"{dbFactory.DelimitIdentifier(colName)} = @{colName}");
            parameters.Add(CreateMappingParameter(colName, colName));
        }

        string sql = $"delete from {table.TableName} where {string.Join(" and ", keyColumns)}";
        return PrepareCommand(sql, parameters, connection, transaction);
    }

    private DbParameter CreateMappingParameter(string paramName, string columnName)
    {
        var p = dbFactory.CreateParameter();
        p.ParameterName = paramName;
        p.SourceColumn = columnName;
        return p;
    }

    private DbCommand PrepareCommand(string sql, IEnumerable<DbParameter> parameters, DbConnection connection, DbTransaction transaction)
    {
        var cmd = dbFactory.CreateCommand(sql, connection, commandTimeout);
        if (transaction != null)
        {
            cmd.Transaction = transaction;
        }
        cmd.Prepare();
        if (parameters?.Count() > 0)
        {
            cmd.Parameters.AddRange(parameters.ToArray());
        }
        return cmd;
    }
}
