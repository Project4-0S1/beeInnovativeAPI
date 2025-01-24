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
    public class StatusController : ControllerBase
    {
        private IUnitOfWork _uow;

        public StatusController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/Status
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Status>>> GetStatus()
        {
            var statuses = await _uow.StatusRepository.GetAllAsync();
            return statuses.ToList();
        }

        // GET: api/Status/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Status>> GetStatus(int id)
        {
            var status = await _uow.StatusRepository.GetByIDAsync(id);

            if (status == null)
            {
                return NotFound();
            }

            return status;
        }

        // PUT: api/Status/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStatus(int id, Status status)
        {
            if (id != status.Id)
            {
                return BadRequest();
            }

            _uow.StatusRepository.Update(status);

            try
            {
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatusExists(id))
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

        // POST: api/Status
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Status>> PostStatus(Status status)
        {
            _uow.StatusRepository.Insert(status);
            await _uow.SaveAsync();

            return CreatedAtAction("GetStatus", new { id = status.Id }, status);
        }

        // DELETE: api/Status/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _uow.StatusRepository.GetByIDAsync(id);
            if (status == null)
            {
                return NotFound();
            }

            _uow.StatusRepository.Delete(id);
            await _uow.SaveAsync();

            return NoContent();
        }

        private bool StatusExists(int id)
        {
            return _uow.StatusRepository.Get(e => e.Id == id).Any();
        }
    }
}
