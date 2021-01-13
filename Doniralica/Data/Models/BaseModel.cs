using System;

namespace Doniralica.Data.Models
{
    public class BaseModel
    {
        public DateTime Created { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? Modified { get; set; }
        public int? ModifiedUserId { get; set; }
    }
}
