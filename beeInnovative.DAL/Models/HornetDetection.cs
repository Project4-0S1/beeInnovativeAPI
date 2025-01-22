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

        public EstimatedNestLocation CalculateEstimatedNestLocation(HornetDetection hornetDetection, Beehive beehive, float distance) {
            double angleRad = (hornetDetection.Direction + beehive.Angle ?? 0) * (Math.PI / 180);
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
            estimatedNestLocation.HornetId = hornetDetection.HornetId;

            return estimatedNestLocation;
        }

        public NestLocation CalculateNestLocation(IEnumerable<EstimatedNestLocation> estimatedNestLocationsList) {
            var cartesianCoordinates = estimatedNestLocationsList.Select(obj =>
            {
                double latRad = DegreesToRadians(obj.EstimatedLatitude);
                double lonRad = DegreesToRadians(obj.EstimatedLongitude);

                return new
                {
                    X = Math.Cos(latRad) * Math.Cos(lonRad),
                    Y = Math.Cos(latRad) * Math.Sin(lonRad),
                    Z = Math.Sin(latRad)
                };
            });

            // Stap 2: Bereken het gemiddelde van de coördinaten
            double xAvg = cartesianCoordinates.Average(coord => coord.X);
            double yAvg = cartesianCoordinates.Average(coord => coord.Y);
            double zAvg = cartesianCoordinates.Average(coord => coord.Z);

            // Stap 3: Normaliseer de gemiddelde coördinaten
            double magnitude = Math.Sqrt(xAvg * xAvg + yAvg * yAvg + zAvg * zAvg);
            double xNorm = xAvg / magnitude;
            double yNorm = yAvg / magnitude;
            double zNorm = zAvg / magnitude;

            // Stap 4: Zet de genormaliseerde coördinaten terug naar latitude/longitude
            double centroidLatitude = RadiansToDegrees(Math.Asin(zNorm));
            double centroidLongitude = RadiansToDegrees(Math.Atan2(yNorm, xNorm));

            NestLocation nestLocation = new NestLocation();
            nestLocation.Latitude = (float)centroidLatitude;
            nestLocation.Longitude = (float)centroidLongitude;
            nestLocation.StatusId = 1;

            return nestLocation;
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadius = 6371e3; // straal van de aarde in meters

            // Converteer graden naar radialen
            double lat1Rad = DegreesToRadians(lat1);
            double lat2Rad = DegreesToRadians(lat2);
            double deltaLatRad = DegreesToRadians(lat2 - lat1);
            double deltaLonRad = DegreesToRadians(lon2 - lon1);

            // Haversine-formule
            double a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadius * c; // afstand in meters
        }

        private double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
