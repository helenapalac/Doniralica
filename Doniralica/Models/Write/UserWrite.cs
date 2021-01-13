namespace Doniralica.Models.Write
{
    public class UserWrite
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public RoleWrite Role { get; set; }
    }
}
