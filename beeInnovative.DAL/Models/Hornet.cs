using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class Hornet
    {
        public int Id {  get; set; }
        public int? ColorId { get; set; }
        public Color? Color { get; set; }

        public ICollection<EstimatedNestLocation>? estimatedNestLocation { get; set; }
    }
}
