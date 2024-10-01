using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace TT.LibrarySys.DataAccess.Models
{
    public class DbConnectionHelper
    {
        private readonly string _connectionString;
        public DbConnectionHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("LibraryDatabase");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
