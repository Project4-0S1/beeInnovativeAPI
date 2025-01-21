using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class Beehive
    {
        public int Id { get; set; }
        public string BeehiveName { get; set; }
        public double? Angle { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string IotId { get; set; }

        public ICollection<UserBeehive>? UserBeehives { get; set; }
        public ICollection<HornetDetection>? HornetDetections { get; set; }

        public Beehive()
        {
            Angle = 0; // Set default value for Angle
        }
    }
}
