using Doniralica.Enumerators;
using System.Collections.Generic;

namespace Doniralica.Data.Models
{
    public class Protege : BaseModel
    {
        public Protege()
        {
            Wishes = new List<Wish>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public GenderTypes Sex { get; set; }
        public bool Active { get; set; }
        /// <summary>
        /// Udruga, dom ili nešto treće
        /// </summary>
        public int? SubjectId { get; set; }
        public List<Wish> Wishes { get; set; }
    }
}
