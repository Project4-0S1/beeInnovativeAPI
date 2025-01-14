using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class HornetDetection
    {
        public int Id { get; set; }
        public DateTime DetectionTimestamp { get; set; }
        public bool IsMarked {  get; set; }
        public int Direction { get; set; }
        public int HornetId { get; set; }
        public int BeehiveId { get; set; }
        
        public Hornet? Hornet { get; set; }
        public Beehive? Beehive { get; set; }
    }
}
