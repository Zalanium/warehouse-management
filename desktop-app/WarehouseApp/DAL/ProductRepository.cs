using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using WarehouseApp.Models;

namespace WarehouseApp.DAL
{
    /// <summary>
    /// Data Access Layer for Product operations
    /// </summary>
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository()
        {
            _connectionString = DatabaseConfig.GetConnectionString();
        }

        /// <summary>
        /// Get all active products
        /// </summary>
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM products WHERE is_active = 1 ORDER BY product_name";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(MapReaderToProduct(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving products: {ex.Message}", ex);
            }

            return products;
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        public Product GetProductById(int productId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM products WHERE product_id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", productId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToProduct(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Search products by name or code
        /// </summary>
        public List<Product> SearchProducts(string searchTerm)
        {
            var products = new List<Product>();

            if (string.IsNullOrWhiteSpace(searchTerm))
                return products;

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"SELECT * FROM products 
                                     WHERE is_active = 1 
                                     AND (product_name LIKE @search OR product_code LIKE @search)
                                     ORDER BY product_name";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@search", $"%{searchTerm}%");

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(MapReaderToProduct(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching products: {ex.Message}", ex);
            }

            return products;
        }

        /// <summary>
        /// Insert new product
        /// </summary>
        public int InsertProduct(Product product)
        {
            ValidateProduct(product);

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO products 
                                    (product_code, product_name, description, category_id, supplier_id, unit_price, min_stock_level, max_stock_level, is_active)
                                    VALUES 
                                    (@code, @name, @desc, @cat, @sup, @price, @min, @max, @active)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@code", product.ProductCode);
                        command.Parameters.AddWithValue("@name", product.ProductName);
                        command.Parameters.AddWithValue("@desc", product.Description ?? "");
                        command.Parameters.AddWithValue("@cat", product.CategoryId);
                        command.Parameters.AddWithValue("@sup", product.SupplierId);
                        command.Parameters.AddWithValue("@price", product.UnitPrice);
                        command.Parameters.AddWithValue("@min", product.MinStockLevel);
                        command.Parameters.AddWithValue("@max", product.MaxStockLevel);
                        command.Parameters.AddWithValue("@active", product.IsActive);

                        command.ExecuteNonQuery();
                        return (int)command.LastInsertedId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting product: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update existing product
        /// </summary>
        public void UpdateProduct(Product product)
        {
            ValidateProduct(product);

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE products SET 
                                    product_name = @name,
                                    description = @desc,
                                    category_id = @cat,
                                    supplier_id = @sup,
                                    unit_price = @price,
                                    min_stock_level = @min,
                                    max_stock_level = @max,
                                    is_active = @active
                                    WHERE product_id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", product.ProductName);
                        command.Parameters.AddWithValue("@desc", product.Description ?? "");
                        command.Parameters.AddWithValue("@cat", product.CategoryId);
                        command.Parameters.AddWithValue("@sup", product.SupplierId);
                        command.Parameters.AddWithValue("@price", product.UnitPrice);
                        command.Parameters.AddWithValue("@min", product.MinStockLevel);
                        command.Parameters.AddWithValue("@max", product.MaxStockLevel);
                        command.Parameters.AddWithValue("@active", product.IsActive);
                        command.Parameters.AddWithValue("@id", product.ProductId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete product (soft delete - mark as inactive)
        /// </summary>
        public void DeleteProduct(int productId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE products SET is_active = 0 WHERE product_id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", productId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product: {ex.Message}", ex);
            }
        }

        private Product MapReaderToProduct(MySqlDataReader reader)
        {
            return new Product
            {
                ProductId = (int)reader["product_id"],
                ProductCode = reader["product_code"].ToString(),
                ProductName = reader["product_name"].ToString(),
                Description = reader["description"].ToString(),
                CategoryId = (int)reader["category_id"],
                SupplierId = (int)reader["supplier_id"],
                UnitPrice = (decimal)reader["unit_price"],
                MinStockLevel = (int)reader["min_stock_level"],
                MaxStockLevel = (int)reader["max_stock_level"],
                IsActive = (bool)reader["is_active"],
                CreatedAt = (DateTime)reader["created_at"],
                UpdatedAt = (DateTime)reader["updated_at"]
            };
        }

        private void ValidateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.ProductCode))
                throw new ArgumentException("Product code is required");

            if (string.IsNullOrWhiteSpace(product.ProductName))
                throw new ArgumentException("Product name is required");

            if (product.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative");

            if (product.MinStockLevel < 0)
                throw new ArgumentException("Minimum stock level cannot be negative");
        }
    }
}
