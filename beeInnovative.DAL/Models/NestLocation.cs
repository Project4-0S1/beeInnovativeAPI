using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class NestLocation
    {
        public int Id { get; set; }
        public float EstimatedLatitude { get; set; }
        public float EstimatedLongitude { get; set; }
        public int HornetId { get; set; }

        public Hornet? Hornet { get; set; }
    }
}
