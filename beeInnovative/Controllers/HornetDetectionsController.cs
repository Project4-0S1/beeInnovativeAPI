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
            var hornetDetections = await _uow.HornetDetectionRepository.GetAllAsync(hd => hd.Hornet);
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
            _uow.HornetDetectionRepository.Insert(hornetDetection);
            await _uow.SaveAsync();

            return CreatedAtAction("GetHornetDetection", new { id = hornetDetection.Id }, hornetDetection);
        }

        // DELETE: api/HornetDetections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHornetDetection(int id)
        {
            var hornetDetection = await _uow.ColorRepository.GetByIDAsync(id);
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
