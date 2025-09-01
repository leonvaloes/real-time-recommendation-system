using CleanArch.Domain.Validation;
using System;

namespace CleanArch.Domain.Entities
{
    public sealed class Purchase
    {
        public int Id { get; private set; }
        public string ProductName { get; private set; }
        public string Category { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public DateTime PurchaseDate { get; private set; }

        public Purchase(int id, string productName, string category, decimal price, int quantity)
        {
            ValidateDomain( productName, category, price, quantity);
            Id = id;
            ProductName = productName;
            Category = category;
            Price = price;
            Quantity = quantity;
            PurchaseDate = DateTime.UtcNow;
        }

        private void ValidateDomain( string productName, string category, decimal price, int quantity)
        {
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(productName), "ProductName is required");
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(category), "Category is required");
            DomainExceptionValidation.When(price <= 0, "Price must be greater than 0");
            DomainExceptionValidation.When(quantity <= 0, "Quantity must be greater than 0");
        }
    }
}
