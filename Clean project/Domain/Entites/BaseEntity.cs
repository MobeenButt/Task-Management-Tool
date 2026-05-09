using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public abstract class BaseEntity
    { 
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
        public int DeletedBy { get; set; }
    }
}
