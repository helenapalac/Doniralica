using Doniralica.Enumerators;
using System.Collections.Generic;

namespace Doniralica.Models.Read
{
    public class ProtegeRead
    {
        public ProtegeRead()
        {
            Wishes = new List<WishRead>();
        }

        public int Id { get; set; }
        public string Initials { get; set; }
        public int Age { get; set; }
        public GenderTypes Sex { get; set; }
        public List<WishRead> Wishes { get; set; }
    }
}
