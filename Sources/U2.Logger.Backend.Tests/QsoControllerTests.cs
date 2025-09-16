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

    [TestMethod]
    public async Task PostQSO_WithValidData_ReturnsCreatedResponseAndStoresCorrectly()
    {
        // Arrange
        var newQso = new QSO
        {
            Band = "17M",
            BandRx = "17M",
            Callsign = "N2CKH",
            Comment = "This is a test comment.",
            Continent = "NA",
            Country = "United States",
            CqZone = "5",
            Distance = 1200,
            Dxcc = "291",
            Email = "n2ckh@qrz.com",
            Freq = 18.123,
            FreqRx = 18.123,
            Gridsquare = "FN31",
            ItuZone = "8",
            Lat = "40.7128",
            Lon = "-74.0060",
            Mode = "FT8",
            MyCity = "New York",
            MyCountry = "United States",
            MyCqZone = "5",
            MyGridsquare = "FN31",
            MyItuZone = "8",
            MyLat = "40.7128",
            MyLon = "-74.0060",
            MyName = "John Doe",
            Name = "Jane Doe",
            Operator = "N2CKH",
            QslRcvd = "Y",
            QslSent = "Y",
            QslVia = "LOTW",
            DateTime = DateTime.UtcNow.ToString("o"),
            DateTimeOff = null,
            Qth = "New York, NY",
            RstRcvd = "599",
            RstSent = "599",
            StationCallsign = "W2XYZ"
        };

        // Act
        var result = await _controller.PostQSO(newQso);

        // Assert that the result is a CreatedAtActionResult.
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);

        // Verify that the returned object is a QSO and has a valid ID.
        var returnedQso = createdResult.Value as QSO;
        Assert.IsNotNull(returnedQso);
        Assert.AreNotEqual(0, returnedQso.Id);

        // Now, retrieve the QSO from the database using the new ID.
        var storedQso = await _context.QSOs.FindAsync(returnedQso.Id);
        Assert.IsNotNull(storedQso, "The QSO was not found in the database.");

        // Assert that all fields were correctly stored.
        Assert.AreEqual(newQso.Band, storedQso.Band);
        Assert.AreEqual(newQso.BandRx, storedQso.BandRx);
        Assert.AreEqual(newQso.Callsign, storedQso.Callsign);
        Assert.AreEqual(newQso.Comment, storedQso.Comment);
        Assert.AreEqual(newQso.Continent, storedQso.Continent);
        Assert.AreEqual(newQso.Country, storedQso.Country);
        Assert.AreEqual(newQso.CqZone, storedQso.CqZone);
        Assert.AreEqual(newQso.Distance, storedQso.Distance);
        Assert.AreEqual(newQso.Dxcc, storedQso.Dxcc);
        Assert.AreEqual(newQso.Email, storedQso.Email);
        Assert.AreEqual(newQso.Freq, storedQso.Freq);
        Assert.AreEqual(newQso.FreqRx, storedQso.FreqRx);
        Assert.AreEqual(newQso.Gridsquare, storedQso.Gridsquare);
        Assert.AreEqual(newQso.ItuZone, storedQso.ItuZone);
        Assert.AreEqual(newQso.Lat, storedQso.Lat);
        Assert.AreEqual(newQso.Lon, storedQso.Lon);
        Assert.AreEqual(newQso.Mode, storedQso.Mode);
        Assert.AreEqual(newQso.MyCity, storedQso.MyCity);
        Assert.AreEqual(newQso.MyCountry, storedQso.MyCountry);
        Assert.AreEqual(newQso.MyCqZone, storedQso.MyCqZone);
        Assert.AreEqual(newQso.MyGridsquare, storedQso.MyGridsquare);
        Assert.AreEqual(newQso.MyItuZone, storedQso.MyItuZone);
        Assert.AreEqual(newQso.MyLat, storedQso.MyLat);
        Assert.AreEqual(newQso.MyLon, storedQso.MyLon);
        Assert.AreEqual(newQso.MyName, storedQso.MyName);
        Assert.AreEqual(newQso.Name, storedQso.Name);
        Assert.AreEqual(newQso.Operator, storedQso.Operator);
        Assert.AreEqual(newQso.QslRcvd, storedQso.QslRcvd);
        Assert.AreEqual(newQso.QslSent, storedQso.QslSent);
        Assert.AreEqual(newQso.QslVia, storedQso.QslVia);
        Assert.AreEqual(newQso.DateTime, storedQso.DateTime);
        Assert.AreEqual(newQso.DateTimeOff, storedQso.DateTimeOff);
        Assert.AreEqual(newQso.Qth, storedQso.Qth);
        Assert.AreEqual(newQso.RstRcvd, storedQso.RstRcvd);
        Assert.AreEqual(newQso.RstSent, storedQso.RstSent);
        Assert.AreEqual(newQso.StationCallsign, storedQso.StationCallsign);
    }
}
