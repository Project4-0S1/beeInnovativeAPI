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

namespace beeInnovative.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HornetDetectionsController : ControllerBase
    {
        private IUnitOfWork _uow;

        public HornetDetectionsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/HornetDetections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HornetDetection>>> GetHornetDetections()
        {
            var hornetDetections = await _uow.HornetDetectionRepository.GetAllAsync(hd => hd.Hornet, b => b.Beehive);
            return hornetDetections.ToList();
        }

        // GET: api/HornetDetections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HornetDetection>> GetHornetDetection(int id)
        {
            var hornetDetection = await _uow.HornetDetectionRepository.GetByIDAsync(id);

            if (hornetDetection == null)
            {
                return NotFound();
            }

            return hornetDetection;
        }

        // PUT: api/HornetDetections/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHornetDetection(int id, HornetDetection hornetDetection)
        {
            if (id != hornetDetection.Id)
            {
                return BadRequest();
            }

            _uow.HornetDetectionRepository.Update(hornetDetection);

            try
            {
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HornetDetectionExists(id))
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

        // POST: api/HornetDetections
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HornetDetection>> PostHornetDetection(HornetDetection hornetDetection)
        {
            IEnumerable<HornetDetection> hornetDetectionList = await _uow.HornetDetectionRepository.GetAllAsync(b => b.Beehive);
            Beehive beehive = await _uow.BeehiveRepository.GetByIDAsync(hornetDetection.BeehiveId);
            hornetDetectionList = hornetDetectionList.Where(b => b.BeehiveId == hornetDetection.BeehiveId).Where(h => h.HornetId == hornetDetection.HornetId).OrderBy(h => h.DetectionTimestamp);

            if (hornetDetectionList.Count() > 0) {
                DateTime newest = new DateTime(2023, 3, 20);
                HornetDetection hornetDetectionCalculation = new HornetDetection();

                foreach (HornetDetection hornetDetectionL in hornetDetectionList)
                {
                    if (hornetDetectionL.DetectionTimestamp > newest)
                    {
                        newest = hornetDetectionL.DetectionTimestamp;
                        hornetDetectionCalculation = hornetDetectionL;
                    }
                }

                float timeBetween = (float)((hornetDetection.DetectionTimestamp - hornetDetectionCalculation.DetectionTimestamp).TotalSeconds / 2);
                float distance = (float)(timeBetween * (30 / 3.6));

                if (distance < 2050) {
                    float angleRad = (float)(hornetDetection.Direction * Math.PI / 180);
                    float latitudeRad = (float)(beehive.Latitude * Math.PI / 180);
                    float longitudeRad = (float)(beehive.Longitude * Math.PI / 180);

                    float earthRadius = 6371000;

                    float newLatitudeRad = (float)Math.Asin(
                        Math.Sin(latitudeRad) * Math.Cos(distance / earthRadius) +
                        Math.Cos(latitudeRad) * Math.Sin(distance / earthRadius) * Math.Cos(angleRad)
                    );

                    float newLongitudeRad = (float)(longitudeRad + Math.Atan2(
                        Math.Sin(angleRad) * Math.Sin(distance / earthRadius) * Math.Cos(latitudeRad),
                        Math.Cos(distance / earthRadius) - Math.Sin(latitudeRad) * Math.Sin(newLatitudeRad)
                    ));

                    // Convert the results back to degrees
                    float newLatitude = (float)(newLatitudeRad * 180 / Math.PI);
                    float newLongitude = (float)(newLongitudeRad * 180 / Math.PI);

                    EstimatedNestLocation estimatedNestLocation = new EstimatedNestLocation();
                    estimatedNestLocation.EstimatedLatitude = newLatitude;
                    estimatedNestLocation.EstimatedLongitude = newLongitude;
                    estimatedNestLocation.HornetId = hornetDetection.HornetId;

                    _uow.EstimatedNestLocationRepository.Insert(estimatedNestLocation);
                }
            }

            _uow.HornetDetectionRepository.Insert(hornetDetection);
            await _uow.SaveAsync();

            return CreatedAtAction("GetHornetDetection", new { id = hornetDetection.Id }, hornetDetection);
        }

        // DELETE: api/HornetDetections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHornetDetection(int id)
        {
            var hornetDetection = await _uow.HornetDetectionRepository.GetByIDAsync(id);
            if (hornetDetection == null)
            {
                return NotFound();
            }

            _uow.HornetDetectionRepository.Delete(id);
            await _uow.SaveAsync();

            return NoContent();
        }

        private bool HornetDetectionExists(int id)
        {
            return _uow.HornetDetectionRepository.Get(e => e.Id == id).Any();
        }
    }
}
