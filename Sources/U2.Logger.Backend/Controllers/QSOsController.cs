using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using U2.Logger.Backend.Data;
using U2.Logger.Backend.Models;

namespace U2.Logger.Backend.Controllers;

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
    public async Task<ActionResult<IEnumerable<QSO>>> GetQSOs()
    {
        return await _context.QSOs.ToListAsync();
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

        return CreatedAtAction("GetQSO", new { id = qso.Id }, qso);
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
