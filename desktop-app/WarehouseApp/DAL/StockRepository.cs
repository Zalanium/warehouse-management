using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using WarehouseApp.Models;

namespace WarehouseApp.DAL
{
    /// <summary>
    /// Data Access Layer for Stock operations
    /// </summary>
    public class StockRepository
    {
        private readonly string _connectionString;

        public StockRepository()
        {
            _connectionString = DatabaseConfig.GetConnectionString();
        }

        /// <summary>
        /// Get current stock for a product
        /// </summary>
        public Stock GetStockByProductId(int productId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM stock WHERE product_id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", productId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToStock(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving stock: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Get all low stock items
        /// </summary>
        public List<Stock> GetLowStockItems()
        {
            var stocks = new List<Stock>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"SELECT s.* FROM stock s
                                     JOIN products p ON s.product_id = p.product_id
                                     WHERE p.is_active = 1 AND s.quantity < p.min_stock_level
                                     ORDER BY s.quantity ASC";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                stocks.Add(MapReaderToStock(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving low stock items: {ex.Message}", ex);
            }

            return stocks;
        }

        /// <summary>
        /// Update stock quantity
        /// </summary>
        public void UpdateStockQuantity(int productId, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidOperationException("Stock quantity cannot be negative");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE stock SET quantity = @qty WHERE product_id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@qty", newQuantity);
                        command.Parameters.AddWithValue("@id", productId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating stock: {ex.Message}", ex);
            }
        }

        private Stock MapReaderToStock(MySqlDataReader reader)
        {
            return new Stock
            {
                StockId = (int)reader["stock_id"],
                ProductId = (int)reader["product_id"],
                LocationId = (int)reader["location_id"],
                Quantity = (int)reader["quantity"],
                LastCountedAt = reader["last_counted_at"] != DBNull.Value ? (DateTime?)reader["last_counted_at"] : null,
                CreatedAt = (DateTime)reader["created_at"],
                UpdatedAt = (DateTime)reader["updated_at"]
            };
        }
    }
}
