using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class EstimatedNestLocation
    {
        public int Id { get; set; }
        public double EstimatedLatitude { get; set; }
        public double EstimatedLongitude { get; set; }
        public int? HornetId { get; set; }

        public Hornet? Hornet { get; set; }

        public EstimatedNestLocation CalculateEstimatedNestLocation(int angle, Beehive beehive, double distance)
        {
            double angleRad = (angle + beehive.Angle ?? 0) * (Math.PI / 180);
            double latitudeRad = beehive.Latitude * Math.PI / 180;
            double longitudeRad = beehive.Longitude * Math.PI / 180;

            float earthRadius = 6371000;

            double newLatitudeRad = Math.Asin(
                Math.Sin(latitudeRad) * Math.Cos(distance / earthRadius) +
                Math.Cos(latitudeRad) * Math.Sin(distance / earthRadius) * Math.Cos(angleRad)
            );

            double newLongitudeRad = longitudeRad + Math.Atan2(
                Math.Sin(angleRad) * Math.Sin(distance / earthRadius) * Math.Cos(latitudeRad),
                Math.Cos(distance / earthRadius) - Math.Sin(latitudeRad) * Math.Sin(newLatitudeRad)
            );

            // Convert the results back to degrees
            double newLatitude = newLatitudeRad * 180 / Math.PI;
            double newLongitude = newLongitudeRad * 180 / Math.PI;

            EstimatedNestLocation estimatedNestLocation = new EstimatedNestLocation();
            estimatedNestLocation.EstimatedLatitude = newLatitude;
            estimatedNestLocation.EstimatedLongitude = newLongitude;
            estimatedNestLocation.HornetId = 4;

            return estimatedNestLocation;
        }
    }
}
