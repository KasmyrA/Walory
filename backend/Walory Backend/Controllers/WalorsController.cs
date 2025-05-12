using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastracture;
using Walory_Backend;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Walory_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalorsController : ControllerBase
    {
        private readonly DataContext _context;

        public WalorsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Walors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Walor>>> GetWalors()
        {
            return await _context.Walors.ToListAsync();
        }

        // GET: api/Walors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Walor>> GetWalor(int id)
        {
            var walor = await _context.Walors.FindAsync(id);

            if (walor == null)
            {
                return NotFound();
            }

            return walor;
        }

        // GET: api/Walors/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Walor>>> GetWalorsByCategory(string category)
        {
            return await _context.Walors
                .Where(w => w.Category == category)
                .ToListAsync();
        }

        // POST: api/Walors
        [HttpPost]
        public async Task<ActionResult<Walor>> CreateWalor(Walor walor)
        {
            _context.Walors.Add(walor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWalor), new { id = walor.Id }, walor);
        }

        // PUT: api/Walors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWalor(int id, Walor walor)
        {
            if (id != walor.Id)
            {
                return BadRequest();
            }

            _context.Entry(walor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WalorExists(id))
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

        // DELETE: api/Walors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWalor(int id)
        {
            var walor = await _context.Walors.FindAsync(id);
            if (walor == null)
            {
                return NotFound();
            }

            _context.Walors.Remove(walor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WalorExists(int id)
        {
            return _context.Walors.Any(e => e.Id == id);
        }
    }
}
