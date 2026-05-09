namespace Domain.Entites
{
    public class User: BaseEntity
    {
        public int Id { get; set; }

        public string? Username { get; set; }
        public string? PasswordHash { get; set; }

        public string? Role { get; set; } // Admin / User
    }
}
