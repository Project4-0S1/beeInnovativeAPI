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

            _uow.EstimatedNestLocationRepository.Insert(estimatedNestLocation);
            await _uow.SaveAsync();

            return Ok();
        }
        
    }
}
