using CleanArch.Domain.Validation;

namespace CleanArch.Domain.Entities
{
    public sealed class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string? Phone { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? LastActivity { get; private set; }
        public IEnumerable<Purchase>? PurchaseHistory { get; private set; }
        public User(string name, string email, string? phone = null)
        {   
            ValidateDomain(name, email, phone);
            Name = name;
            Email = email;
            Phone = phone;
        }
        public User(int id, string name, string email, string? phone = null)
        {
            ValidateDomain(name, email, phone);

            Id = id;
            Name = name;
            Email = email;
            Phone = phone;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        private void ValidateDomain(string Name, string Email, string? Phone)
        {
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(Name), "Name is required");
            DomainExceptionValidation.When(Name.Length < 2, "Name too short");
            DomainExceptionValidation.When(Name.Length > 150, "Name too long");

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(Email), "Email is required");

            if (!string.IsNullOrEmpty(Phone))
            {
                DomainExceptionValidation.When(Phone.Length < 8, "Phone too short");
                DomainExceptionValidation.When(Phone.Length > 20, "Phone too long");
            }
        }

    }

}
