using Microsoft.AspNetCore.Mvc;
using U2.Logger.Backend.Data;
using U2.Logger.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace U2.Logger.Backend.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class QSOsController : ControllerBase
    {
        private readonly LoggerContext _context;

        public QSOsController(LoggerContext context)
        {
            _context = context;
        }

        // GET: api/v1/QSOs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QSO>>> GetQSOs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string sortKey = "dateTime",
            [FromQuery] string sortDirection = "desc",
            [FromQuery] string callsignContains = null)
        {
            var query = _context.QSOs.AsQueryable();

            // Apply filter if a callsign fragment is provided
            if (!string.IsNullOrWhiteSpace(callsignContains))
            {
                query = query.Where(q => q.Callsign.ToLower().Contains(callsignContains.ToLower()));
            }

            // Get the total number of records before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            switch (sortKey.ToLower())
            {
                case "callsign":
                    query = (sortDirection.ToLower() == "asc") ? query.OrderBy(q => q.Callsign) : query.OrderByDescending(q => q.Callsign);
                    break;
                case "band":
                    query = (sortDirection.ToLower() == "asc") ? query.OrderBy(q => q.Band) : query.OrderByDescending(q => q.Band);
                    break;
                case "mode":
                    query = (sortDirection.ToLower() == "asc") ? query.OrderBy(q => q.Mode) : query.OrderByDescending(q => q.Mode);
                    break;
                case "datetime":
                default:
                    query = (sortDirection.ToLower() == "asc") ? query.OrderBy(q => q.DateTime) : query.OrderByDescending(q => q.DateTime);
                    break;
            }

            // Apply pagination
            var qsos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Add the total count to the response headers for the frontend to use
            Response.Headers.Add("X-Total-Count", totalCount.ToString());

            return Ok(qsos);
        }

        // GET: api/v1/QSOs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QSO>> GetQSO(int id)
        {
            var qso = await _context.QSOs.FindAsync(id);
            if (qso == null)
            {
                return NotFound();
            }
            return qso;
        }

        // POST: api/v1/QSOs
        [HttpPost]
        public async Task<ActionResult<QSO>> PostQSO(QSO qso)
        {
            _context.QSOs.Add(qso);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQSOs", new { id = qso.Id }, qso);
        }

        // PUT: api/v1/QSOs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQSO(int id, QSO qso)
        {
            if (id != qso.Id)
            {
                return BadRequest();
            }

            _context.Entry(qso).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.QSOs.Any(e => e.Id == id))
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

        // DELETE: api/v1/QSOs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQSO(int id)
        {
            var qso = await _context.QSOs.FindAsync(id);
            if (qso == null)
            {
                return NotFound();
            }

            _context.QSOs.Remove(qso);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
