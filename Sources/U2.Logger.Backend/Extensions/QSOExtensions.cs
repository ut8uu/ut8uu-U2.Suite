namespace U2.Logger.Backend.Extensions;

/// <summary>
/// A static class containing extension methods for converting between
/// the core QSO DTO and the backend's persistence model.
/// </summary>
public static class QSOExtensions
{
    /// <summary>
    /// Converts a core QSO DTO to a backend persistence model.
    /// This is used when receiving data from the frontend to save to the database.
    /// </summary>
    /// <param name="qsoDto">The QSO DTO from the shared library.</param>
    /// <returns>A new backend QSO model ready for database interaction.</returns>
    public static Backend.Models.QSO ToBackendModel(this Core.Models.QSO qsoDto)
    {
        return new Backend.Models.QSO
        {
            Id = qsoDto.Id,
            Band = qsoDto.Band,
            BandRx = qsoDto.BandRx,
            Callsign = qsoDto.Callsign,
            Comment = qsoDto.Comment,
            Continent = qsoDto.Continent,
            Country = qsoDto.Country,
            CqZone = qsoDto.CqZone,
            DateTime = qsoDto.DateTime,
            DateTimeOff = qsoDto.DateTimeOff,
            Distance = qsoDto.Distance,
            Dxcc = qsoDto.Dxcc,
            Email = qsoDto.Email,
            Freq = qsoDto.Freq,
            FreqRx = qsoDto.FreqRx,
            Gridsquare = qsoDto.Gridsquare,
            ItuZone = qsoDto.ItuZone,
            Lat = qsoDto.Lat,
            Lon = qsoDto.Lon,
            Mode = qsoDto.Mode,
            MyCity = qsoDto.MyCity,
            MyCountry = qsoDto.MyCountry,
            MyCqZone = qsoDto.MyCqZone,
            MyGridsquare = qsoDto.MyGridsquare,
            MyItuZone = qsoDto.MyItuZone,
            MyLat = qsoDto.MyLat,
            MyLon = qsoDto.MyLon,
            MyName = qsoDto.MyName,
            Name = qsoDto.Name,
            Operator = qsoDto.Operator,
            QslRcvd = qsoDto.QslRcvd,
            QslSent = qsoDto.QslSent,
            QslVia = qsoDto.QslVia,
            Qth = qsoDto.Qth,
            RstRcvd = qsoDto.RstRcvd,
            RstSent = qsoDto.RstSent,
            StationCallsign = qsoDto.StationCallsign
        };
    }

    /// <summary>
    /// Converts a backend QSO model to a core QSO DTO.
    /// This is used when retrieving data from the database to send back to the frontend.
    /// </summary>
    /// <param name="qsoBackend">The backend QSO model with database annotations.</param>
    /// <returns>A new core QSO DTO, free of database-specific logic.</returns>
    public static Core.Models.QSO ToDto(this Backend.Models.QSO qsoBackend)
    {
        return new Core.Models.QSO
        {
            Id = qsoBackend.Id,
            Band = qsoBackend.Band,
            BandRx = qsoBackend.BandRx,
            Callsign = qsoBackend.Callsign,
            Comment = qsoBackend.Comment,
            Continent = qsoBackend.Continent,
            Country = qsoBackend.Country,
            CqZone = qsoBackend.CqZone,
            DateTime = qsoBackend.DateTime,
            DateTimeOff = qsoBackend.DateTimeOff,
            Distance = qsoBackend.Distance,
            Dxcc = qsoBackend.Dxcc,
            Email = qsoBackend.Email,
            Freq = qsoBackend.Freq,
            FreqRx = qsoBackend.FreqRx,
            Gridsquare = qsoBackend.Gridsquare,
            ItuZone = qsoBackend.ItuZone,
            Lat = qsoBackend.Lat,
            Lon = qsoBackend.Lon,
            Mode = qsoBackend.Mode,
            MyCity = qsoBackend.MyCity,
            MyCountry = qsoBackend.MyCountry,
            MyCqZone = qsoBackend.MyCqZone,
            MyGridsquare = qsoBackend.MyGridsquare,
            MyItuZone = qsoBackend.MyItuZone,
            MyLat = qsoBackend.MyLat,
            MyLon = qsoBackend.MyLon,
            MyName = qsoBackend.MyName,
            Name = qsoBackend.Name,
            Operator = qsoBackend.Operator,
            QslRcvd = qsoBackend.QslRcvd,
            QslSent = qsoBackend.QslSent,
            QslVia = qsoBackend.QslVia,
            Qth = qsoBackend.Qth,
            RstRcvd = qsoBackend.RstRcvd,
            RstSent = qsoBackend.RstSent,
            StationCallsign = qsoBackend.StationCallsign
        };
    }
}
