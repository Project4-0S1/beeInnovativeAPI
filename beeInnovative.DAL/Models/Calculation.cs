using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class Calculation
    {
        public int Id { get; set; }
        public int BeehiveId { get; set; }
        public int TimeBetween { get; set; }
        public int Direction { get; set; }
    }
}
