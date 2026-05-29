using System;
using System.Configuration;

namespace WarehouseApp.DAL
{
    /// <summary>
    /// Database connection configuration and management
    /// </summary>
    public static class DatabaseConfig
    {
        private static readonly string _connectionString;

        static DatabaseConfig()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WarehouseDb"]?.ConnectionString 
                ?? "Server=localhost;Database=warehouse_management;Uid=root;Pwd=root;";
        }

        public static string GetConnectionString() => _connectionString;
    }
}
