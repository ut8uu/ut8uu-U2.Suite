using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using U2.Logger.Backend.Controllers;
using U2.Logger.Backend.Data;
using U2.Logger.Backend.Models;

namespace U2.Logger.Backend.Tests;

[TestClass]
public sealed class QsoControllerTests
{
    private LoggerContext _context;
    private QSOsController _controller;

    [TestInitialize]
    public void Initialize()
    {
        // Use a unique database name for each test run to ensure complete isolation.
        var databaseName = Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder<LoggerContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;
        _context = new LoggerContext(options);
        _context.Database.EnsureDeleted();

        _controller = new QSOsController(_context)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            }
        };

        _context.QSOs.Add(new QSO
        {
            Id = 1,
            DateTime = new DateTime(2025, 9, 15, 12, 0, 0).ToString("o"),
            Band = "20M",
            Callsign = "W1AW",
            Freq = 14.250,
            Mode = "SSB",
            Operator = "U2",
            RstRcvd = "59",
            RstSent = "59"
        });
        _context.QSOs.Add(new QSO
        {
            Id = 2,
            DateTime = new DateTime(2025, 9, 15, 12, 30, 0).ToString("o"),
            Band = "40M",
            Callsign = "K9AT",
            Freq = 7.150,
            Mode = "CW",
            Operator = "U2",
            RstRcvd = "599",
            RstSent = "599"
        });
        _context.SaveChanges();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    [DataRow(1, 1, "callsign", "asc", "w1aw", 1, "W1AW", DisplayName = "Filter and sort by callsign (ascending)")]
    [DataRow(1, 1, "callsign", "desc", "k9at", 1, "K9AT", DisplayName = "Filter and sort by callsign (descending)")]
    [DataRow(1, 2, "band", "asc", null, 2, "W1AW", DisplayName = "Sort by band (ascending)")]
    [DataRow(1, 2, "band", "desc", null, 2, "K9AT", DisplayName = "Sort by band (descending)")]
    [DataRow(1, 2, "datetime", "asc", null, 2, "W1AW", DisplayName = "Paginate and sort by datetime (ascending)")]
    [DataRow(1, 2, "datetime", "desc", null, 2, "K9AT", DisplayName = "Paginate and sort by datetime (descending)")]
    public async Task GetQSOs_WithSortingFilteringAndPagination_ReturnsCorrectData(
        int pageNumber,
        int pageSize,
        string sortKey,
        string sortDirection,
        string callsignContains,
        int expectedCount,
        string expectedFirstCallsign)
    {
        // Act
        var result = await _controller.GetQSOs(pageNumber, pageSize, sortKey, sortDirection, callsignContains);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var qsos = okResult.Value as IEnumerable<QSO>;
        Assert.IsNotNull(qsos);
        Assert.AreEqual(expectedCount, qsos.Count());
        Assert.AreEqual(expectedFirstCallsign, qsos.First().Callsign);

        // Verify that the X-Total-Count header was added and has the correct value.
        Assert.IsTrue(_controller.Response.Headers.ContainsKey("X-Total-Count"));
        Assert.AreEqual(expectedCount.ToString(), _controller.Response.Headers["X-Total-Count"].ToString());
    }
}
