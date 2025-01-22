using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class EstimatedNestLocation
    {
        public int Id { get; set; }
        public double EstimatedLatitude { get; set; }
        public double EstimatedLongitude { get; set; }
        public int? HornetId { get; set; }

        public Hornet? Hornet { get; set; }
    }
}
