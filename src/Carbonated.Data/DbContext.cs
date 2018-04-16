using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Carbonated.Data
{
    public interface DbContext : IDisposable
    {
        DbObjectFactory ObjectFactory { get; }

        int NonQuery(string sql, IEnumerable<DbParameter> parameters = null);
        int NonQuery(string sql, params (string name, object value)[] parameters);

        IEnumerable<TEntity> Query<TEntity>(string sql, params (string name, object value)[] parameters);
        IEnumerable<TEntity> Query<TEntity>(string sql, IEnumerable<DbParameter> parameters = null);

        EntityReader<TEntity> QueryReader<TEntity>(string sql, params (string name, object value)[] parameters);
        EntityReader<TEntity> QueryReader<TEntity>(string sql, IEnumerable<DbParameter> parameters = null);
        DbDataReader QueryReader(string sql, params (string name, object value)[] parameters);
        DbDataReader QueryReader(string sql, IEnumerable<DbParameter> parameters = null);

        TResult QueryScalar<TResult>(string sql, params (string name, object value)[] parameters);
        TResult QueryScalar<TResult>(string sql, IEnumerable<DbParameter> parameters = null);
        object QueryScalar(string sql, params (string name, object value)[] parameters);
        object QueryScalar(string sql, IEnumerable<DbParameter> parameters = null);

        DataTable QueryTable(string sql, params (string name, object value)[] parameters);
        DataTable QueryTable(string sql, IEnumerable<DbParameter> parameters);
        int SaveTable(DataTable table);
    }
}