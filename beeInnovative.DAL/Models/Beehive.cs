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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string IotId { get; set; }
        public DateTime? lastCall { get; set; }

        public ICollection<UserBeehive>? UserBeehives { get; set; }
        public ICollection<HornetDetection>? HornetDetections { get; set; }

        public Beehive()
        {
            Angle = 0; // Set default value for Angle
        }
    }
}
