using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using beeInnovative.DAL.Models;

namespace beeInnovative.DAL.Data
{
    public class DBInitializer
    {
        public static void Initialize(BeeInnovativeContext context)
        {
            context.Database.EnsureCreated();

            // Look for any products.
            if (context.Beehives.Any())
            {
                return;   // DB has been seeded
            }

            //Add Beehives
            Beehive b1 = new Beehive()
            {
                //Id = 1,
                BeehiveName = "Beehive1",
                Latitude = 12.3456f,
                Longitude = 78.9012f,
                IotId = 1,
            };

            Beehive b2 = new Beehive()
            {
                //Id = 2,
                BeehiveName = "Beehive2",
                Latitude = 23.4567f,
                Longitude = 89.0123f,
                IotId = 2,
            };

            context.Add(b1);
            context.Add(b2);

            //Add Colors
            Color c1 = new Color()
            {
                //Id = 1,
                ColorName = "red",
            };

            Color c2 = new Color()
            {
                //Id = 2,
                ColorName = "Blue",
            };

            context.Add(c1);
            context.Add(c2);

            //Add Hornets
            Hornet h1 = new Hornet()
            {
                //Id = 1,
                ColorId = 1
            };

            Hornet h2 = new Hornet()
            {
                //Id = 2,
                ColorId = 2
            };

            context.Add(h1);
            context.Add(h2);


            //Add Users
            User u1 = new User()
            {
                //Id = 1,
                UserSubTag = "User1",
            };

            User u2 = new User()
            {
                //Id = 2,
                UserSubTag = "User2",
            };

            context.Add(u1);
            context.Add(u2);

            //Add UserBeehives
            UserBeehive ub1 = new UserBeehive()
            {
                //Id = 1,
                BeehiveId = 1,
                UserId = 1
            };

            UserBeehive ub2 = new UserBeehive()
            {
                //Id = 2,
                BeehiveId = 2,
                UserId = 2,
            };

            context.Add(ub1);
            context.Add(ub2);

            //Add NestLocations
            NestLocation nl1 = new NestLocation()
            {
                //Id = 1,
                EstimatedLatitude = 12.3456f,
                EstimatedLongitude = 78.9012f,
                HornetId = 1,
            };

            NestLocation nl2 = new NestLocation()
            {
                //Id = 2,
                EstimatedLatitude = 23.4567f,
                EstimatedLongitude = 89f,
                HornetId = 2,
            };
            context.Add(nl1);
            context.Add(nl2);

            context.SaveChanges();
        }
    }
}
