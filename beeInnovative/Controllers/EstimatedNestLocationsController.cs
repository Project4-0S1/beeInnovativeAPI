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
    public class EstimatedNestLocationsController : ControllerBase
    {
        private IUnitOfWork _uow;

        public EstimatedNestLocationsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/NestLocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstimatedNestLocation>>> GetEstimatedNestLocations()
        {
            var nestLocations = await _uow.EstimatedNestLocationRepository.GetAllAsync(n => n.Hornet);
            return nestLocations.ToList();
        }

        // GET: api/NestLocations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EstimatedNestLocation>> GetEstimatedNestLocation(int id)
        {
            var nestLocation = await _uow.EstimatedNestLocationRepository.GetByIDAsync(id);

            if (nestLocation == null)
            {
                return NotFound();
            }

            return nestLocation;
        }

        // PUT: api/NestLocations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstimatedNestLocation(int id, EstimatedNestLocation nestLocation)
        {
            if (id != nestLocation.Id)
            {
                return BadRequest();
            }

            _uow.EstimatedNestLocationRepository.Update(nestLocation);

            try
            {
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NestEstimatedLocationExists(id))
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

        // POST: api/NestLocations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EstimatedNestLocation>> PostEstimatedNestLocation(EstimatedNestLocation nestLocation)
        {
            _uow.EstimatedNestLocationRepository.Insert(nestLocation);
            await _uow.SaveAsync();

            return CreatedAtAction("GetNestLocation", new { id = nestLocation.Id }, nestLocation);
        }

        // DELETE: api/NestLocations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstimatedNestLocation(int id)
        {
            var nestLocation = await _uow.EstimatedNestLocationRepository.GetByIDAsync(id);
            if (nestLocation == null)
            {
                return NotFound();
            }

            _uow.ColorRepository.Delete(id);
            await _uow.SaveAsync();

            return NoContent();
        }

        private bool NestEstimatedLocationExists(int id)
        {
            return _uow.ColorRepository.Get(e => e.Id == id).Any();
        }
    }
}
