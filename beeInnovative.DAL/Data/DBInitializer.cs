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
                BeehiveName = "De C",
                Latitude = 51.164120751116435f,
                Longitude = 4.961838518509023f,
                IotId = "1",
            };

            Beehive b2 = new Beehive()
            {
                //Id = 2,
                BeehiveName = "Pizza Hut",
                Latitude = 51.14956154963047f,
                Longitude = 4.964806904144326f,
                IotId = "2",
            };

            context.Add(b1);
            context.Add(b2);

            //Add Colors
            Color c1 = new Color()
            {
                //Id = 1,
                ColorName = "Red",
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
                EstimatedLatitude = 51.16080291398481f,
                EstimatedLongitude = 4.9644732260095275f,
                HornetId = 1,
            };

            NestLocation nl2 = new NestLocation()
            {
                //Id = 2,
                EstimatedLatitude = 51.170209175807024f,
                EstimatedLongitude = 4.968067403732371f,
                HornetId = 2,
            };
            context.Add(nl1);
            context.Add(nl2);

            context.SaveChanges();
        }
    }
}
