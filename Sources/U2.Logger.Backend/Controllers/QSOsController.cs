using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using U2.Logger.Backend.Data;
using U2.Logger.Backend.Extensions;
using U2.Logger.Backend.Models;
using U2.Logger.Backend.Services;
using U2.Logger.Core.Models;

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

    /// <summary>
    /// Retrieves a paginated and sortable list of QSOs.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="sortKey">The field to sort by.</param>
    /// <param name="sortDirection">The sort direction (asc or desc).</param>
    /// <param name="callsignContains">Optional filter to search for callsigns containing a specific string.</param>
    /// <returns>A list of QSOs.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Core.Models.QSO>>> GetQSOs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sortKey = "DateTime",
        [FromQuery] string sortDirection = "desc",
        [FromQuery] string? callsignContains = null)
    {
        IQueryable<Models.QSO> query = _context.QSOs;

        if (!string.IsNullOrEmpty(callsignContains))
        {
            query = query.Where(q => q.Callsign.ToLower().Contains(callsignContains.ToLower()));
        }

        // Apply sorting dynamically based on the sortKey and sortDirection
        switch (sortKey.ToLower())
        {
            case "datetime":
                query = sortDirection.ToLower() == "desc" ? query.OrderByDescending(q => q.DateTime) : query.OrderBy(q => q.DateTime);
                break;
            case "callsign":
                query = sortDirection.ToLower() == "desc" ? query.OrderByDescending(q => q.Callsign) : query.OrderBy(q => q.Callsign);
                break;
            case "band":
                query = sortDirection.ToLower() == "desc" ? query.OrderByDescending(q => q.Band) : query.OrderBy(q => q.Band);
                break;
            // Add more cases for other sortable properties as needed
            default:
                // Default to sorting by DateTime
                query = sortDirection.ToLower() == "desc" ? query.OrderByDescending(q => q.DateTime) : query.OrderBy(q => q.DateTime);
                break;
        }

        var totalCount = await query.CountAsync();
        Response.Headers.Append("X-Total-Count", totalCount.ToString());

        var qsos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var resultingList = qsos.Select(q => q.ToDto()).ToList();

        return Ok(resultingList);
    }

    /// <summary>
    /// Retrieves a single QSO by its ID.
    /// </summary>
    /// <param name="id">The ID of the QSO.</param>
    /// <returns>The QSO with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Core.Models.QSO>> GetQSO(int id)
    {
        if (_context.QSOs == null)
        {
            return NotFound();
        }

        var qso = await _context.QSOs.FindAsync(id);

        if (qso == null)
        {
            return NotFound();
        }

        return qso.ToDto();
    }

    /// <summary>
    /// Updates a specific QSO.
    /// </summary>
    /// <param name="id">The ID of the QSO to update.</param>
    /// <param name="qso">The updated QSO object.</param>
    /// <returns>A NoContent response if successful, or NotFound if the QSO does not exist.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutQSO(int id, Core.Models.QSO qso)
    {
        if (!QSOValidator.IsValid(qso, out var message))
        {
            return BadRequest(message);
        }

        if (id != qso?.Id)
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
            if (!QSOExists(id))
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

    /// <summary>
    /// Creates a new QSO.
    /// </summary>
    /// <param name="qso">The QSO object to be created.</param>
    /// <returns>The newly created QSO object.</returns>
    [HttpPost]
    public async Task<ActionResult<Core.Models.QSO>> PostQSO(Core.Models.QSO qsoDto)
    {
        if (!QSOValidator.IsValid(qsoDto, out var message))
        {
            return BadRequest(message);
        }

        // Use the extension method to convert the DTO to the backend model
        var qsoBackend = qsoDto.ToBackendModel();

        _context.QSOs.Add(qsoBackend);
        await _context.SaveChangesAsync();

        // Optionally, return the converted DTO back to the client
        return CreatedAtAction("GetQSO", new { id = qsoBackend.Id }, qsoBackend.ToDto());
    }

    /// <summary>
    /// Deletes a specific QSO.
    /// </summary>
    /// <param name="id">The ID of the QSO to delete.</param>
    /// <returns>A NoContent response if successful, or NotFound if the QSO does not exist.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQSO(int id)
    {
        if (_context.QSOs == null)
        {
            return NotFound();
        }

        var qso = await _context.QSOs.FindAsync(id);
        if (qso == null)
        {
            return NotFound();
        }

        _context.QSOs.Remove(qso);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool QSOExists(int id)
    {
        return (_context.QSOs?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
