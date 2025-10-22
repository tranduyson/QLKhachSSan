using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace QLKhachSanApi.DAL
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("HotelDb");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }

}
