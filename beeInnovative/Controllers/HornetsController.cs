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

namespace beeInnovative.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HornetsController : ControllerBase
    {
        private IUnitOfWork _uow;

        public HornetsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/Hornets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hornet>>> GetHornets()
        {
            var hornets = await _uow.HornetRepository.GetAllAsync(h => h.Color);
            return hornets.ToList(); ;
        }

        // GET: api/Hornets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hornet>> GetHornet(int id)
        {
            var hornet = await _uow.HornetRepository.GetByIDAsync(id);

            if (hornet == null)
            {
                return NotFound();
            }

            return hornet;
        }

        // PUT: api/Hornets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHornet(int id, Hornet hornet)
        {
            if (id != hornet.Id)
            {
                return BadRequest();
            }

            _uow.HornetRepository.Update(hornet);

            try
            {
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HornetExists(id))
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

        // POST: api/Hornets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hornet>> PostHornet(Hornet hornet)
        {
            _uow.HornetRepository.Insert(hornet);
            await _uow.SaveAsync();

            return CreatedAtAction("GetHornet", new { id = hornet.Id }, hornet);
        }

        // DELETE: api/Hornets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHornet(int id)
        {
            var hornet = await _uow.HornetRepository.GetByIDAsync(id);
            if (hornet == null)
            {
                return NotFound();
            }

            _uow.HornetRepository.Delete(id);
            await _uow.SaveAsync();

            return NoContent();
        }

        private bool HornetExists(int id)
        {
            return _uow.HornetRepository.Get(e => e.Id == id).Any();
        }
    }
}
