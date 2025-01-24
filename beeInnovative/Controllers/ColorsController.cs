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
    public class ColorsController : ControllerBase
    {
        private IUnitOfWork _uow;

        public ColorsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/Colors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Color>>> GetColors()
        {
            var colors = await _uow.ColorRepository.GetAllAsync(c => c.Hornet);
            return colors.ToList();
        }

        // GET: api/Colors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Color>> GetColor(int id)
        {
            var color = await _uow.ColorRepository.GetByIDAsync(id);

            if (color == null)
            {
                return NotFound();
            }

            return color;
        }

        // PUT: api/Colors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutColor(int id, Color color)
        {
            if (id != color.Id)
            {
                return BadRequest();
            }

            _uow.ColorRepository.Update(color);

            try
            {
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColorExists(id))
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

        // POST: api/Colors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Color>> PostColor(Color color)
        {
            _uow.ColorRepository.Insert(color);
            await _uow.SaveAsync();

            return CreatedAtAction("GetColor", new { id = color.Id }, color);
        }

        // DELETE: api/Colors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            var color = await _uow.ColorRepository.GetByIDAsync(id);
            if (color == null)
            {
                return NotFound();
            }

            _uow.ColorRepository.Delete(id);
            await _uow.SaveAsync();

            return NoContent();
        }

        private bool ColorExists(int id)
        {
            return _uow.ColorRepository.Get(e => e.Id == id).Any();
        }
    }
}
