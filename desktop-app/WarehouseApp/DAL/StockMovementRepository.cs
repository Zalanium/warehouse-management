using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using WarehouseApp.Models;

namespace WarehouseApp.DAL
{
    /// <summary>
    /// Data Access Layer for Stock Movement operations
    /// </summary>
    public class StockMovementRepository
    {
        private readonly string _connectionString;

        public StockMovementRepository()
        {
            _connectionString = DatabaseConfig.GetConnectionString();
        }

        /// <summary>
        /// Get movement history for a product
        /// </summary>
        public List<StockMovement> GetMovementHistory(int productId, int? limit = null)
        {
            var movements = new List<StockMovement>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM stock_movements WHERE product_id = @id ORDER BY created_at DESC";
                    
                    if (limit.HasValue)
                        query += $" LIMIT {limit.Value}";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", productId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                movements.Add(MapReaderToMovement(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving movement history: {ex.Message}", ex);
            }

            return movements;
        }

        /// <summary>
        /// Record a stock movement
        /// </summary>
        public void RecordMovement(StockMovement movement)
        {
            if (movement == null)
                throw new ArgumentNullException(nameof(movement));

            if (movement.Quantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO stock_movements 
                                    (product_id, movement_type, quantity, reason, old_quantity, new_quantity, created_by)
                                    VALUES 
                                    (@prod, @type, @qty, @reason, @old, @new, @user)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@prod", movement.ProductId);
                        command.Parameters.AddWithValue("@type", movement.MovementType);
                        command.Parameters.AddWithValue("@qty", movement.Quantity);
                        command.Parameters.AddWithValue("@reason", movement.Reason ?? "");
                        command.Parameters.AddWithValue("@old", movement.OldQuantity ?? 0);
                        command.Parameters.AddWithValue("@new", movement.NewQuantity ?? 0);
                        command.Parameters.AddWithValue("@user", movement.CreatedBy ?? "system");

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error recording movement: {ex.Message}", ex);
            }
        }

        private StockMovement MapReaderToMovement(MySqlDataReader reader)
        {
            return new StockMovement
            {
                MovementId = (int)reader["movement_id"],
                ProductId = (int)reader["product_id"],
                MovementType = reader["movement_type"].ToString(),
                Quantity = (int)reader["quantity"],
                Reason = reader["reason"].ToString(),
                OldQuantity = reader["old_quantity"] != DBNull.Value ? (int?)reader["old_quantity"] : null,
                NewQuantity = reader["new_quantity"] != DBNull.Value ? (int?)reader["new_quantity"] : null,
                CreatedBy = reader["created_by"].ToString(),
                CreatedAt = (DateTime)reader["created_at"]
            };
        }
    }
}
