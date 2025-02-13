using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using beeInnovative.DAL.Data;
using beeInnovative.DAL.Models;
using beeInnovative.DAL.Service;
using System.Drawing;
using Azure.Core;

namespace beeInnovative.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculationsController : ControllerBase
    {
        private IUnitOfWork _uow;

        public CalculationsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/Colors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Calculation>>> GetCalculations()
        {
            var calculations = await _uow.CalculationRepository.GetAllAsync(c => c.Hornet);
            return calculations.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Calculation>> GetCalculations(int id)
        {
            var calculation = await _uow.CalculationRepository.GetByIDAsync(id);

            if (calculation == null)
            {
                return NotFound();
            }

            return calculation;
        }

        // PUT: api/Colors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutColor(int id, Calculation calculation)
        {
            if (id != calculation.Id)
            {
                return BadRequest();
            }

            _uow.CalculationRepository.Update(calculation);

            try
            {
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CalculationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Calculations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Calculation>> PostCalculation(Calculation calculation)
        {
            Beehive beehive = await _uow.BeehiveRepository.GetByIDAsync(calculation.BeehiveId);
            if (beehive == null)
            {
                return NotFound($"Beehive with ID {calculation.BeehiveId} not found.");
            }
            double distance = (calculation.TimeBetween * (30 / 3.6));
            double angleRad = (calculation.Direction + beehive.Angle ?? 0) * (Math.PI / 180);
            double latitudeRad = beehive.Latitude * Math.PI / 180;
            double longitudeRad = beehive.Longitude * Math.PI / 180;

            double earthRadius = 6371000;

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

            IEnumerable<EstimatedNestLocation> estimatedNestLocationsList = await _uow.EstimatedNestLocationRepository.GetAllAsync();

            _uow.EstimatedNestLocationRepository.Insert(estimatedNestLocation);
            await _uow.SaveAsync();

            foreach (EstimatedNestLocation estimatedNest in estimatedNestLocationsList)
            {
                double distanceBetween = CalculateDistance(
                    estimatedNestLocation.EstimatedLatitude,
                    estimatedNestLocation.EstimatedLongitude,
                    estimatedNest.EstimatedLatitude,
                    estimatedNest.EstimatedLongitude
                );

                if (distanceBetween <= 120) // controleer of binnen 80 meter
                {
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
                    await _uow.SaveAsync();
                    _uow.EstimatedNestLocationRepository.Delete(estimatedNest.Id);
                    _uow.EstimatedNestLocationRepository.Delete(estimatedNestLocation.Id);
                    _uow.NestLocationRepository.Insert(nestLocation);
                    await _uow.SaveAsync();
                }
            }

            return Ok();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCalculation(int id)
        {
            var calculation = await _uow.CalculationRepository.GetByIDAsync(id);
            if (calculation == null)
            {
                return NotFound();
            }

            _uow.CalculationRepository.Delete(id);
            await _uow.SaveAsync();

            return NoContent();
        }

        private bool CalculationExists(int id)
        {
            return _uow.CalculationRepository.Get(e => e.Id == id).Any();
        }
    }
}
