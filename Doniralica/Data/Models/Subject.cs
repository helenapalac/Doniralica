using System.Collections.Generic;

namespace Doniralica.Data.Models
{
    public class Subject : BaseModel
    {
        public Subject()
        {
            Proteges = new List<Protege>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ResponsiblePersonFullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public List<Protege> Proteges { get; set; }
    }
}
