using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserSubTag { get; set; }

        public ICollection<UserBeehive>? UserBeehives { get; set; }
    }
}
