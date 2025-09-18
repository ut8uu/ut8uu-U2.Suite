using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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

    // Define a static list of known bands for validation.
    private static readonly Dictionary<string, (double min, double max)> KnownBands = new()
    {
        { "160M", (1.8, 2.0) },
        { "80M", (3.5, 4.0) },
        { "60M", (5.3, 5.4) },
        { "40M", (7.0, 7.3) },
        { "30M", (10.1, 10.15) },
        { "20M", (14.0, 14.35) },
        { "17M", (18.068, 18.168) },
        { "15M", (21.0, 21.45) },
        { "12M", (24.89, 24.99) },
        { "10M", (28.0, 29.7) },
        { "6M", (50.0, 54.0) },
        { "2M", (144.0, 148.0) },
        { "70CM", (420.0, 450.0) }
    };

    // Define a static list of known modes for validation.
    private static readonly HashSet<string> KnownModes = new()
    {
        "SSB", "CW", "FM", "AM", "RTTY", "FT8", "PSK31"
    };

    public QSOsController(LoggerContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Validates a QSO object against a set of business rules.
    /// </summary>
    /// <param name="qso">The QSO object to validate.</param>
    /// <param name="message">An output parameter that contains a descriptive error message if validation fails.</param>
    /// <returns>True if the QSO is valid, otherwise false.</returns>
    private static bool ValidateQSO(QSO qso, out string message)
    {
        if (qso == null)
        {
            message = "QSO data is required.";
            return false;
        }

        // Rule 1: Callsign must be 3 or more symbols.
        if (string.IsNullOrWhiteSpace(qso.Callsign) || qso.Callsign.Length < 3)
        {
            message = "Callsign must be 3 or more characters long.";
            return false;
        }

        // Rule 2: DateTime must be in a valid format.
        // We'll use a standard ISO 8601 format ("o") which is used in the default QSO.
        if (!qso.DateTime.HasValue)
        {
            message = "DateTime is a mandatory field.";
            return false;
        }

        // Rule 3: Mode must be present and one of the known values.
        if (string.IsNullOrWhiteSpace(qso.Mode) || !KnownModes.Contains(qso.Mode.ToUpperInvariant()))
        {
            message = $"Invalid or missing Mode. Valid modes are: {string.Join(", ", KnownModes)}.";
            return false;
        }

        // Rule 4: Band or Freq must be present.
        var hasBand = !string.IsNullOrWhiteSpace(qso.Band);
        var hasFreq = qso.Freq.HasValue;

        if (!hasBand && !hasFreq)
        {
            message = "Either 'Band' or 'Freq' must be present.";
            return false;
        }

        // Rule 5: If both Band and Freq are present, they must match.
        if (hasBand && hasFreq)
        {
            if (!KnownBands.TryGetValue(qso.Band.ToUpperInvariant(), out var bandRange))
            {
                message = "Invalid 'Band' value. Cannot validate against frequency.";
                return false;
            }

            if (qso.Freq > 0 && (qso.Freq.Value < bandRange.min || qso.Freq.Value > bandRange.max))
            {
                message = $"Frequency '{qso.Freq}' does not match the specified Band '{qso.Band}'. It should be within the range {bandRange.min}-{bandRange.max} MHz.";
                return false;
            }
        }

        message = string.Empty;
        return true;
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
    public async Task<ActionResult<IEnumerable<QSO>>> GetQSOs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sortKey = "DateTime",
        [FromQuery] string sortDirection = "desc",
        [FromQuery] string? callsignContains = null)
    {
        IQueryable<QSO> query = _context.QSOs;

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

        return Ok(qsos);
    }

    /// <summary>
    /// Retrieves a single QSO by its ID.
    /// </summary>
    /// <param name="id">The ID of the QSO.</param>
    /// <returns>The QSO with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<QSO>> GetQSO(int id)
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

        return qso;
    }

    /// <summary>
    /// Updates a specific QSO.
    /// </summary>
    /// <param name="id">The ID of the QSO to update.</param>
    /// <param name="qso">The updated QSO object.</param>
    /// <returns>A NoContent response if successful, or NotFound if the QSO does not exist.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutQSO(int id, QSO qso)
    {
        if (!ValidateQSO(qso, out var message))
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
    public async Task<ActionResult<QSO>> PostQSO([FromBody] QSO qso)
    {
        if (!ValidateQSO(qso, out var message))
        {
            return BadRequest(message);
        }

        if (_context.QSOs == null)
        {
            return Problem("Entity set 'LoggerContext.QSOs' is null.");
        }

        _context.QSOs.Add(qso);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetQSO", new { id = qso.Id }, qso);
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
