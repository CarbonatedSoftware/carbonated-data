using System.Data.Common;
using System.Data.SqlClient;

namespace Carbonated.Data.SqlServer
{
    public class SqlServerObjectFactory : DbObjectFactory
    {
        /// <summary>
        /// Creates a SqlCommand.
        /// </summary>
        /// <returns>The command.</returns>
        public override DbCommand CreateCommand() => new SqlCommand();

        /// <summary>
        /// Creates a SqlConnection for the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The connection with the connection string set.</returns>
        public override DbConnection CreateConnection(string connectionString) => new SqlConnection(connectionString);

        /// <summary>
        /// Creates a SqlDataAdapter.
        /// </summary>
        /// <returns>The data adapter.</returns>
        public override DbDataAdapter CreateDataAdapter() => new SqlDataAdapter();

        /// <summary>
        /// Creates a SqlParameter.
        /// </summary>
        /// <returns>The parameter.</returns>
        public override DbParameter CreateParameter() => new SqlParameter();
    }
}
