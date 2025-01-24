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
using Microsoft.AspNetCore.Authorization;

namespace beeInnovative.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BeehivesController : ControllerBase
    {
        private IUnitOfWork _uow;

        public BeehivesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/Beehives
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Beehive>>> GetBeehives(
    [FromQuery] bool? filterByUserBeehives = null)
        {
            // Get all beehives along with UserBeehives and HornetDetections
            var beehives = await _uow.BeehiveRepository.GetAllAsync(b => b.HornetDetections, b => b.UserBeehives);

            // Apply filtering if the flag is set
            if (filterByUserBeehives.HasValue && filterByUserBeehives.Value)
            {
                // Only return beehives that do not have any associated UserBeehives
                beehives = beehives.Where(b => b.UserBeehives == null || !b.UserBeehives.Any()).ToList();
            }

            return Ok(beehives);
        }


        // GET: api/Beehives/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Beehive>> GetBeehive(int id)
        {
            var beehive = await _uow.BeehiveRepository.GetByIDAsync(id);

            if (beehive == null)
            {
                return NotFound();
            }

            return beehive;
        }

        [HttpGet("iot/{iotId}")]
        public async Task<ActionResult<Beehive>> GetBeehiveByIotId(string iotId)
        {
            IEnumerable<Beehive> beehives = await _uow.BeehiveRepository.GetAllAsync();
            Beehive beehive = beehives.Where(b => b.IotId == iotId).First();

            if (beehive == null)
            {
                return NotFound();
            }

            return beehive;
        }

        // PUT: api/Beehives/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBeehive(int id, Beehive beehive)
        {
            if (id != beehive.Id)
            {
                return BadRequest();
            }

            _uow.BeehiveRepository.Update(beehive);

            try
            {
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeehiveExists(id))
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

        // POST: api/Beehives
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Beehive>> PostBeehive(Beehive beehive)
        {
            IEnumerable<Beehive> beehivesList = await _uow.BeehiveRepository.GetAllAsync();

            foreach(Beehive beehiveOne in beehivesList)
            {
                if (beehiveOne.IotId == beehive.IotId) {
                    _uow.BeehiveRepository.Update(beehive);
                    await _uow.SaveAsync();
                }
            }

            _uow.BeehiveRepository.Insert(beehive);
            await _uow.SaveAsync();

            return CreatedAtAction("GetBeehive", new { id = beehive.Id }, beehive);
        }

        // DELETE: api/Beehives/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBeehive(int id)
        {
            var beehive = await _uow.BeehiveRepository.GetByIDAsync(id);
            if (beehive == null)
            {
                return NotFound();
            }

            _uow.BeehiveRepository.Delete(id);
            await _uow.SaveAsync();

            return NoContent();
        }

        private bool BeehiveExists(int id)
        {
            return _uow.BeehiveRepository.Get(e => e.Id == id).Any();
        }
    }
}
