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
                Latitude = 51.164120751116435,
                Longitude = 4.961838518509023,
                IotId = "1",
                lastCall = DateTime.Now,
            };

            Beehive b2 = new Beehive()
            {
                //Id = 2,
                BeehiveName = "Pizza Hut",
                Latitude = 51.14956154963047,
                Longitude = 4.964806904144326,
                IotId = "2",
                lastCall = DateTime.Now,
            };

            Beehive b3 = new Beehive()
            {
                //Id = 1,
                BeehiveName = "",
                Latitude = 51.161736776231265,
                Longitude = 5.003174937424427,
                IotId = "3",
            };

            Beehive b4 = new Beehive()
            {
                //Id = 2,
                BeehiveName = "",
                Latitude = 51.134813494164845,
                Longitude = 4.941647521836946,
                IotId = "4",
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

            Color c3 = new Color()
            {
                //Id = 2,
                ColorName = "Green",
            };

            Color c4 = new Color()
            {
                //Id = 2,
                ColorName = "Unknown",
            };

            context.Add(c1);
            context.Add(c2);
            context.Add(c3);
            context.Add(c4);


            Status s2 = new Status()
            {
                type = "Detected"
            };

            Status s1 = new Status()
            {
                type = "Found"
            };

            Status s3 = new Status()
            {
                type = "Cleared"
            };

            context.Add(s2);
            context.Add(s1);
            context.Add(s3);

            context.SaveChanges();

            //Add Hornets
            Hornet h1 = new Hornet()
            {
                ColorId = 1
            };

            Hornet h2 = new Hornet()
            {
                ColorId = 2
            };

            Hornet h3 = new Hornet()
            {
                ColorId = 3
            };

            Hornet h4 = new Hornet()
            {
                ColorId = 4
            };

            context.Add(h1);
            context.Add(h2);
            context.Add(h3);
            context.Add(h4);

            context.SaveChanges();

            //Add NestLocations
            EstimatedNestLocation nl1 = new EstimatedNestLocation()
            {
                //Id = 1,
                EstimatedLatitude = 51.16080291398481,
                EstimatedLongitude = 4.9644732260095275,
                HornetId = 1,
            };

            EstimatedNestLocation nl2 = new EstimatedNestLocation()
            {
                //Id = 2,
                EstimatedLatitude = 51.170209175807024,
                EstimatedLongitude = 4.968067403732371,
                HornetId = 2,
            };
            context.Add(nl1);
            context.Add(nl2);

            context.SaveChanges();
        }
    }
}
