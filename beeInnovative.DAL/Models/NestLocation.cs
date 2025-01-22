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
        public int StatusId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Status? status { get; set; }

    }
}
