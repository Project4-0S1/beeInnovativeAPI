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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace beeInnovative.Controllers
{
    [Authorize]
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
        public async Task<ActionResult<IEnumerable<HornetDetection>>> GetHornetDetections(int? beehiveId = null)
        {
            if (beehiveId.HasValue)
            {
                // Fetch hornet detections filtered by the specified beehive ID
                var hornetDetections = await _uow.HornetDetectionRepository.GetAllAsync(hd => hd.Hornet, b => b.Beehive, c => c.Hornet.Color);
                hornetDetections = hornetDetections.Where(h => h.BeehiveId == beehiveId);
                return hornetDetections.ToList();
            }
            else
            {
                // Fetch all hornet detections if no beehive ID is specified
                var hornetDetections = await _uow.HornetDetectionRepository.GetAllAsync(hd => hd.Hornet, b => b.Beehive);
                return hornetDetections.ToList();
            }
        }

        // GET: api/HornetDetections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HornetDetection>> GetHornetDetection(int id)
        {
            // Retrieve a specific hornet detection by ID
            var hornetDetection = await _uow.HornetDetectionRepository.GetByIDAsync(id);

            if (hornetDetection == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return hornetDetection;
        }

        // PUT: api/HornetDetections/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHornetDetection(int id, HornetDetection hornetDetection)
        {
            if (id != hornetDetection.Id)
            {
                return BadRequest(); // Return 400 if ID mismatch
            }

            _uow.HornetDetectionRepository.Update(hornetDetection);

            try
            {
                await _uow.SaveAsync(); // Attempt to save changes
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues
                if (!HornetDetectionExists(id))
                {
                    return NotFound(); // Return 404 if not found
                }
                else
                {
                    throw; // Rethrow the exception if it's a different issue
                }
            }

            return NoContent(); // Return 204 No Content on success
        }

        // POST: api/HornetDetections
        [HttpPost]
        public async Task<ActionResult<HornetDetection>> PostHornetDetection(HornetDetection hornetDetection)
        {
            // Fetch existing hornet detections and the corresponding beehive
            IEnumerable<HornetDetection> hornetDetectionList = await _uow.HornetDetectionRepository.GetAllAsync(b => b.Beehive);
            Beehive beehive = await _uow.BeehiveRepository.GetByIDAsync(hornetDetection.BeehiveId);
            IEnumerable<HornetDetection> filteredHornetDetectionList = hornetDetectionList
                .Where(b => b.BeehiveId == hornetDetection.BeehiveId)
                .Where(h => h.HornetId == hornetDetection.HornetId)
                .OrderBy(h => h.DetectionTimestamp);

            if (hornetDetectionList.Count() > 0)
            {
                DateTime newest = new DateTime(2023, 3, 20);
                HornetDetection hornetDetectionCalculation = new HornetDetection();

                // Find the most recent detection
                foreach (HornetDetection hornetDetectionL in filteredHornetDetectionList)
                {
                    if (hornetDetectionL.DetectionTimestamp > newest)
                    {
                        newest = hornetDetectionL.DetectionTimestamp;
                        hornetDetectionCalculation = hornetDetectionL;
                    }
                }

                // Calculate time and distance based on the most recent detection
                float timeBetween = (float)((hornetDetection.DetectionTimestamp - hornetDetectionCalculation.DetectionTimestamp).TotalSeconds / 2);
                float distance = (float)(timeBetween * (30 / 3.6));

                if (distance < 2050)
                {
                    // Calculate estimated nest location if within distance threshold
                    EstimatedNestLocation estimatedNestLocation = hornetDetection.CalculateEstimatedNestLocation(hornetDetection, beehive, distance);
                    IEnumerable<EstimatedNestLocation> estimatedNestLocationsList = await _uow.EstimatedNestLocationRepository.GetAllAsync();
                    _uow.EstimatedNestLocationRepository.Insert(estimatedNestLocation);

                    // Check for existing estimated nest locations
                    foreach (EstimatedNestLocation estimatedNest in estimatedNestLocationsList)
                    {
                        double distanceBetween = hornetDetection.CalculateDistance(
                            estimatedNestLocation.EstimatedLatitude,
                            estimatedNestLocation.EstimatedLongitude,
                            estimatedNest.EstimatedLatitude,
                            estimatedNest.EstimatedLongitude
                        );

                        if (distanceBetween <= 120) // Check if within 120 meters
                        {
                            NestLocation nestLocation = hornetDetection.CalculateNestLocation([estimatedNest, estimatedNestLocation]);
                            await _uow.SaveAsync(); // Save changes
                            _uow.EstimatedNestLocationRepository.Delete(estimatedNest.Id); // Remove old estimated nest
                            _uow.EstimatedNestLocationRepository.Delete(estimatedNestLocation.Id); // Remove new estimated nest
                            _uow.NestLocationRepository.Insert(nestLocation); // Insert new nest location
                            await _uow.SaveAsync(); // Save changes
                        }
                    }
                }
            }

            _uow.HornetDetectionRepository.Insert(hornetDetection); // Insert new hornet detection
            await _uow.SaveAsync(); // Save changes

            return CreatedAtAction("GetHornetDetection", new { id = hornetDetection.Id }, hornetDetection); // Return 201 Created
        }

        // POST: api/HornetDetections/multiple
        [HttpPost("multiple")]
        public async Task<ActionResult<IEnumerable<HornetDetectionRasp>>> MultiplePostHornetDetection(IEnumerable<HornetDetectionRasp> hornetDetections)
        {
            if (hornetDetections.Count() > 0)
            {
                foreach (HornetDetectionRasp hornetDetection in hornetDetections)
                {
                    // Fetch existing hornet detections and beehives
                    IEnumerable<HornetDetection> hornetDetectionList = await _uow.HornetDetectionRepository.GetAllAsync(b => b.Beehive);
                    IEnumerable<Beehive> beehives = await _uow.BeehiveRepository.GetAllAsync();

                    // Find the corresponding beehive by IoT ID
                    Beehive beehive = beehives.Where(b => b.IotId == hornetDetection.BeehiveId).First();

                    HornetDetection hornetDetectionCorrect = new HornetDetection
                    {
                        DetectionTimestamp = hornetDetection.DetectionTimestamp,
                        IsMarked = hornetDetection.IsMarked,
                        Direction = hornetDetection.Direction,
                        HornetId = hornetDetection.HornetId,
                        BeehiveId = beehive.Id
                    };

                    // Filter existing detections for the current beehive and hornet
                    IEnumerable<HornetDetection> filteredHornetDetectionList = hornetDetectionList
                        .Where(b => b.BeehiveId == hornetDetectionCorrect.BeehiveId)
                        .Where(h => h.HornetId == hornetDetection.HornetId)
                        .OrderBy(h => h.DetectionTimestamp);

                    if (hornetDetectionList.Count() > 0)
                    {
                        DateTime newest = new DateTime(2023, 3, 20);
                        HornetDetection hornetDetectionCalculation = new HornetDetection();

                        // Find the most recent detection
                        foreach (HornetDetection hornetDetectionL in filteredHornetDetectionList)
                        {
                            if (hornetDetectionL.DetectionTimestamp > newest)
                            {
                                newest = hornetDetectionL.DetectionTimestamp;
                                hornetDetectionCalculation = hornetDetectionL;
                            }
                        }

                        // Calculate time and distance based on the most recent detection
                        float timeBetween = (float)((hornetDetection.DetectionTimestamp - hornetDetectionCalculation.DetectionTimestamp).TotalSeconds / 2);
                        float distance = (float)(timeBetween * (30 / 3.6));

                        if (distance < 2050)
                        {
                            // Calculate estimated nest location if within distance threshold
                            EstimatedNestLocation estimatedNestLocation = hornetDetection.CalculateEstimatedNestLocation(hornetDetectionCorrect, beehive, distance);
                            IEnumerable<EstimatedNestLocation> estimatedNestLocationsList = await _uow.EstimatedNestLocationRepository.GetAllAsync();
                            _uow.EstimatedNestLocationRepository.Insert(estimatedNestLocation);

                            // Check for existing estimated nest locations
                            foreach (EstimatedNestLocation estimatedNest in estimatedNestLocationsList)
                            {
                                double distanceBetween = hornetDetection.CalculateDistance(
                                    estimatedNestLocation.EstimatedLatitude,
                                    estimatedNestLocation.EstimatedLongitude,
                                    estimatedNest.EstimatedLatitude,
                                    estimatedNest.EstimatedLongitude
                                );

                                if (distanceBetween <= 120) // Check if within 120 meters
                                {
                                    NestLocation nestLocation = hornetDetection.CalculateNestLocation([estimatedNest, estimatedNestLocation]);
                                    await _uow.SaveAsync(); // Save changes
                                    _uow.EstimatedNestLocationRepository.Delete(estimatedNest.Id); // Remove old estimated nest
                                    _uow.EstimatedNestLocationRepository.Delete(estimatedNestLocation.Id); // Remove new estimated nest
                                    _uow.NestLocationRepository.Insert(nestLocation); // Insert new nest location
                                    await _uow.SaveAsync(); // Save changes
                                }
                            }
                        }
                    }

                    _uow.HornetDetectionRepository.Insert(hornetDetectionCorrect); // Insert new hornet detection
                    await _uow.SaveAsync(); // Save changes
                }

                return hornetDetections.ToList(); // Return the list of detections
            }

            return BadRequest("Te weinig detections opgegeven"); // Return 400 if no detections provided
        }

        // DELETE: api/HornetDetections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHornetDetection(int id)
        {
            // Retrieve the hornet detection to be deleted
            var hornetDetection = await _uow.HornetDetectionRepository.GetByIDAsync(id);
            if (hornetDetection == null)
            {
                return NotFound(); // Return 404 if not found
            }

            _uow.HornetDetectionRepository.Delete(id); // Delete the detection
            await _uow.SaveAsync(); // Save changes

            return NoContent(); // Return 204 No Content on success
        }

        private bool HornetDetectionExists(int id)
        {
            // Check if a hornet detection exists by ID
            return _uow.HornetDetectionRepository.Get(e => e.Id == id).Any();
        }
    }
}