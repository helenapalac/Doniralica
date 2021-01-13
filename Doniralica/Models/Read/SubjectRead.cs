using System.Collections.Generic;

namespace Doniralica.Models.Read
{
    public class SubjectRead
    {
        public SubjectRead()
        {
            Proteges = new List<ProtegeRead>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ResponsiblePersonFullName { get; set; }
        public string PhoneNumber { get; set; }
        public List<ProtegeRead> Proteges { get; set; }
    }
}
