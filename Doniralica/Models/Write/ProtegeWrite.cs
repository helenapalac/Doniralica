using Doniralica.Enumerators;

namespace Doniralica.Models.Write
{
    public class ProtegeWrite
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public GenderTypes Sex { get; set; }
    }
}
