namespace Domain.Entities
{
    public class AppUser : BaseEntity
    {
        private AppUser()
        {
            // EF Core
        }

        public string KeycloakUserId { get; private set; }

        public string FullName { get; private set; }

        public string Email { get; private set; }

        public string PhoneNumber { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public static AppUser Create(
            string keycloakUserId,
            string fullName,
            string email,
            string phoneNumber = null)
        {
            if (string.IsNullOrWhiteSpace(keycloakUserId))
                throw new ArgumentException("KeycloakUserId is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            return new AppUser
            {
                KeycloakUserId = keycloakUserId,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}