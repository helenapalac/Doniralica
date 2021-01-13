namespace Doniralica.Models.Read
{
    public class UserRead
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public RoleRead Role { get; set; }
    }
}
