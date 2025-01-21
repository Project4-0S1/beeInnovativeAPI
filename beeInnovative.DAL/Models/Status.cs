using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string type { get; set; }

        public ICollection<NestLocation>? nestLocation { get; set; }
    }
}
