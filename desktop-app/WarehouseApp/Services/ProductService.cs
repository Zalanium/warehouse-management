using System;
using System.Collections.Generic;
using WarehouseApp.DAL;
using WarehouseApp.Models;

namespace WarehouseApp.Services
{
    /// <summary>
    /// Business logic layer for Product operations
    /// </summary>
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly StockRepository _stockRepository;

        public ProductService()
        {
            _productRepository = new ProductRepository();
            _stockRepository = new StockRepository();
        }

        /// <summary>
        /// Get all products with their stock information
        /// </summary>
        public List<(Product Product, Stock Stock)> GetAllProductsWithStock()
        {
            var result = new List<(Product, Stock)>();
            var products = _productRepository.GetAllProducts();

            foreach (var product in products)
            {
                var stock = _stockRepository.GetStockByProductId(product.ProductId);
                result.Add((product, stock));
            }

            return result;
        }

        /// <summary>
        /// Search products
        /// </summary>
        public List<Product> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Product>();

            return _productRepository.SearchProducts(searchTerm);
        }

        /// <summary>
        /// Create new product
        /// </summary>
        public int CreateProduct(string productCode, string productName, string description, 
            int categoryId, int supplierId, decimal unitPrice, int minStock, int maxStock)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(productCode))
                throw new ArgumentException("Product code cannot be empty");

            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name cannot be empty");

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative");

            if (minStock < 0)
                throw new ArgumentException("Minimum stock cannot be negative");

            if (maxStock <= minStock)
                throw new ArgumentException("Maximum stock must be greater than minimum stock");

            var product = new Product
            {
                ProductCode = productCode,
                ProductName = productName,
                Description = description,
                CategoryId = categoryId,
                SupplierId = supplierId,
                UnitPrice = unitPrice,
                MinStockLevel = minStock,
                MaxStockLevel = maxStock,
                IsActive = true
            };

            return _productRepository.InsertProduct(product);
        }

        /// <summary>
        /// Update existing product
        /// </summary>
        public void UpdateProduct(int productId, string productName, string description,
            int categoryId, int supplierId, decimal unitPrice, int minStock, int maxStock)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null)
                throw new InvalidOperationException("Product not found");

            product.ProductName = productName;
            product.Description = description;
            product.CategoryId = categoryId;
            product.SupplierId = supplierId;
            product.UnitPrice = unitPrice;
            product.MinStockLevel = minStock;
            product.MaxStockLevel = maxStock;

            _productRepository.UpdateProduct(product);
        }

        /// <summary>
        /// Deactivate product
        /// </summary>
        public void DeactivateProduct(int productId)
        {
            _productRepository.DeleteProduct(productId);
        }
    }
}
