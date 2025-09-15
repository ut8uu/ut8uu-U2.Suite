using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace U2.Logger.Backend.Models;

public class QSO
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("band")]
    public string? Band { get; set; }

    [Column("band_rx")]
    public string? BandRx { get; set; }

    [Column("call")]
    public string? Callsign { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("cont")]
    public string? Continent { get; set; }

    [Column("country")]
    public string? Country { get; set; }

    [Column("cqz")]
    public string? CqZone { get; set; }

    [Column("distance")]
    public int? Distance { get; set; }

    [Column("dxcc")]
    public string? Dxcc { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("freq")]
    public double? Freq { get; set; }

    [Column("freq_rx")]
    public double? FreqRx { get; set; }

    [Column("gridsquare")]
    public string? Gridsquare { get; set; }

    [Column("ituz")]
    public string? ItuZone { get; set; }

    [Column("lat")]
    public string? Lat { get; set; }

    [Column("lon")]
    public string? Lon { get; set; }

    [Column("mode")]
    public string? Mode { get; set; }

    [Column("my_city")]
    public string? MyCity { get; set; }

    [Column("my_country")]
    public string? MyCountry { get; set; }

    [Column("my_cq_zone")]
    public string? MyCqZone { get; set; }

    [Column("my_gridsquare")]
    public string? MyGridsquare { get; set; }

    [Column("my_itu_zone")]
    public string? MyItuZone { get; set; }

    [Column("my_lat")]
    public string? MyLat { get; set; }

    [Column("my_lon")]
    public string? MyLon { get; set; }

    [Column("my_name")]
    public string? MyName { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("operator")]
    public string? Operator { get; set; }

    [Column("qsl_rcvd")]
    public string? QslRcvd { get; set; }

    [Column("qsl_sent")]
    public string? QslSent { get; set; }

    [Column("qsl_via")]
    public string? QslVia { get; set; }

    [Column("date_time")]
    public string? DateTime { get; set; }

    [Column("date_time_off")]
    public string? DateTimeOff { get; set; }

    [Column("qth")]
    public string? Qth { get; set; }

    [Column("rst_rcvd")]
    public string? RstRcvd { get; set; }

    [Column("rst_sent")]
    public string? RstSent { get; set; }

    [Column("station_callsign")]
    public string? StationCallsign { get; set; }
}
