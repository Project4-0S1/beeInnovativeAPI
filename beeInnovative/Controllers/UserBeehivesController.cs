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
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace beeInnovative.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserBeehivesController : ControllerBase
    {
        private IUnitOfWork _uow;

        public UserBeehivesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/UserBeehives
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserBeehive>>> GetUserBeehives()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<User> users = await _uow.UserRepository.GetAllAsync();
            User user = users.Where(u => u.UserSubTag == userId).First();

            IEnumerable<UserBeehive> userBeehives = await _uow.UserBeehiveRepository.GetAllAsync(ub => ub.Beehive, ub => ub.User);
            userBeehives = userBeehives.Where(u => u.User.UserSubTag == user.UserSubTag);

            return userBeehives.ToList();
        }

        // GET: api/UserBeehives/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserBeehive>> GetUserBeehive(int id)
        {
            var userBeehive = await _uow.UserBeehiveRepository.GetByIDAsync(id);

            if (userBeehive == null)
            {
                return NotFound();
            }

            return userBeehive;
        }

        // PUT: api/UserBeehives/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserBeehive(int id, UserBeehive userBeehive)
        {
            if (id != userBeehive.Id)
            {
                return BadRequest();
            }

            _uow.UserBeehiveRepository.Update(userBeehive);

            try
            {
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserBeehiveExists(id))
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

        // POST: api/UserBeehives
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserBeehive>> PostUserBeehive(UserBeehive userBeehive)
        {
            _uow.UserBeehiveRepository.Insert(userBeehive);
            await _uow.SaveAsync();

            return CreatedAtAction("GetUserBeehive", new { id = userBeehive.Id }, userBeehive);
        }

        // DELETE: api/UserBeehives/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserBeehive(int id)
        {
            var userBeehive = await _uow.UserBeehiveRepository.GetByIDAsync(id);
            if (userBeehive == null)
            {
                return NotFound();
            }

            _uow.UserBeehiveRepository.Delete(id);
            await _uow.SaveAsync();

            return NoContent();
        }

        private bool UserBeehiveExists(int id)
        {
            return _uow.UserBeehiveRepository.Get(e => e.Id == id).Any();
        }
    }
}
