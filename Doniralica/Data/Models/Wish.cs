namespace Doniralica.Data.Models
{
    public class Wish : BaseModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int ProtegeId { get; set; }
        public string DonatorName { get; set; }
        public string DonatorPhone { get; set; }
    }
}
