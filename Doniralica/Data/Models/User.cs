namespace Doniralica.Data.Models
{
    public class User : BaseModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public Role Role { get; set; }
        public bool IsAdminUser { get; set; }
    }
}
