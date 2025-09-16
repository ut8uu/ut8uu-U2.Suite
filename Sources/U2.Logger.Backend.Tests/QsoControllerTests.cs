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

    private QSO CreateTestQSO()
    {
        return new QSO
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
    }

    private void CompareQSO(QSO qso1, QSO qso2)
    {
        Assert.AreEqual(qso1.Band, qso2.Band);
        Assert.AreEqual(qso1.BandRx, qso2.BandRx);
        Assert.AreEqual(qso1.Callsign, qso2.Callsign);
        Assert.AreEqual(qso1.Comment, qso2.Comment);
        Assert.AreEqual(qso1.Continent, qso2.Continent);
        Assert.AreEqual(qso1.Country, qso2.Country);
        Assert.AreEqual(qso1.CqZone, qso2.CqZone);
        Assert.AreEqual(qso1.Distance, qso2.Distance);
        Assert.AreEqual(qso1.Dxcc, qso2.Dxcc);
        Assert.AreEqual(qso1.Email, qso2.Email);
        Assert.AreEqual(qso1.Freq, qso2.Freq);
        Assert.AreEqual(qso1.FreqRx, qso2.FreqRx);
        Assert.AreEqual(qso1.Gridsquare, qso2.Gridsquare);
        Assert.AreEqual(qso1.ItuZone, qso2.ItuZone);
        Assert.AreEqual(qso1.Lat, qso2.Lat);
        Assert.AreEqual(qso1.Lon, qso2.Lon);
        Assert.AreEqual(qso1.Mode, qso2.Mode);
        Assert.AreEqual(qso1.MyCity, qso2.MyCity);
        Assert.AreEqual(qso1.MyCountry, qso2.MyCountry);
        Assert.AreEqual(qso1.MyCqZone, qso2.MyCqZone);
        Assert.AreEqual(qso1.MyGridsquare, qso2.MyGridsquare);
        Assert.AreEqual(qso1.MyItuZone, qso2.MyItuZone);
        Assert.AreEqual(qso1.MyLat, qso2.MyLat);
        Assert.AreEqual(qso1.MyLon, qso2.MyLon);
        Assert.AreEqual(qso1.MyName, qso2.MyName);
        Assert.AreEqual(qso1.Name, qso2.Name);
        Assert.AreEqual(qso1.Operator, qso2.Operator);
        Assert.AreEqual(qso1.QslRcvd, qso2.QslRcvd);
        Assert.AreEqual(qso1.QslSent, qso2.QslSent);
        Assert.AreEqual(qso1.QslVia, qso2.QslVia);
        Assert.AreEqual(qso1.DateTime, qso2.DateTime);
        Assert.AreEqual(qso1.DateTimeOff, qso2.DateTimeOff);
        Assert.AreEqual(qso1.Qth, qso2.Qth);
        Assert.AreEqual(qso1.RstRcvd, qso2.RstRcvd);
        Assert.AreEqual(qso1.RstSent, qso2.RstSent);
        Assert.AreEqual(qso1.StationCallsign, qso2.StationCallsign);
    }

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
        var newQso = CreateTestQSO();

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

        CompareQSO(storedQso, newQso);
    }

    [TestMethod]
    public async Task PutQSO_WithValidData_UpdatesQsoCorrectly()
    {
        // Arrange
        var qsoToUpdateId = 1;
        var updatedQso = CreateTestQSO();
        updatedQso.Id = qsoToUpdateId;

        // This is the fix. We need to tell the context to stop tracking the original entity
        // before we try to attach a new one with the same key.
        var originalQso = await _context.QSOs.FindAsync(qsoToUpdateId);
        if (originalQso != null)
        {
            _context.Entry(originalQso).State = EntityState.Detached;
        }

        // Act
        var result = await _controller.PutQSO(qsoToUpdateId, updatedQso);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));

        // Retrieve the QSO from the database and verify the update.
        var storedQso = await _context.QSOs.FindAsync(qsoToUpdateId);
        Assert.IsNotNull(storedQso, "The QSO was not found in the database after the update.");

        // Verify that all fields were correctly updated.
        CompareQSO(updatedQso, storedQso);
    }
}
