using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class UserBeehive
    {
        public int Id { get; set; }
        public int BeehiveId {  get; set; }
        public int UserId { get; set; }
        public Beehive? Beehive { get; set; }
        public User? User { get; set; }
    }
}
