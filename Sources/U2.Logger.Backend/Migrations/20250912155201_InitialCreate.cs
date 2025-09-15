using Microsoft.EntityFrameworkCore.Migrations;

namespace U2.Logger.Backend.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "QSOs",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Band = table.Column<string>(name: "band", type: "TEXT", nullable: true),
                BandRx = table.Column<string>(name: "band_rx", type: "TEXT", nullable: true),
                Callsign = table.Column<string>(name: "call", type: "TEXT", nullable: true),
                Comment = table.Column<string>(name: "comment", type: "TEXT", nullable: true),
                Continent = table.Column<string>(name: "cont", type: "TEXT", nullable: true),
                Country = table.Column<string>(name: "country", type: "TEXT", nullable: true),
                CqZone = table.Column<string>(name: "cqz", type: "TEXT", nullable: true),
                Distance = table.Column<int>(name: "distance", type: "INTEGER", nullable: true),
                Dxcc = table.Column<string>(name: "dxcc", type: "TEXT", nullable: true),
                Email = table.Column<string>(name: "email", type: "TEXT", nullable: true),
                Freq = table.Column<double>(name: "freq", type: "REAL", nullable: true),
                FreqRx = table.Column<double>(name: "freq_rx", type: "REAL", nullable: true),
                Gridsquare = table.Column<string>(name: "gridsquare", type: "TEXT", nullable: true),
                ItuZone = table.Column<string>(name: "ituz", type: "TEXT", nullable: true),
                Lat = table.Column<string>(name: "lat", type: "TEXT", nullable: true),
                Lon = table.Column<string>(name: "lon", type: "TEXT", nullable: true),
                Mode = table.Column<string>(name: "mode", type: "TEXT", nullable: true),
                MyCity = table.Column<string>(name: "my_city", type: "TEXT", nullable: true),
                MyCountry = table.Column<string>(name: "my_country", type: "TEXT", nullable: true),
                MyCqZone = table.Column<string>(name: "my_cq_zone", type: "TEXT", nullable: true),
                MyGridsquare = table.Column<string>(name: "my_gridsquare", type: "TEXT", nullable: true),
                MyItuZone = table.Column<string>(name: "my_itu_zone", type: "TEXT", nullable: true),
                MyLat = table.Column<string>(name: "my_lat", type: "TEXT", nullable: true),
                MyLon = table.Column<string>(name: "my_lon", type: "TEXT", nullable: true),
                MyName = table.Column<string>(name: "my_name", type: "TEXT", nullable: true),
                Name = table.Column<string>(name: "name", type: "TEXT", nullable: true),
                Operator = table.Column<string>(name: "operator", type: "TEXT", nullable: true),
                QslRcvd = table.Column<string>(name: "qsl_rcvd", type: "TEXT", nullable: true),
                QslSent = table.Column<string>(name: "qsl_sent", type: "TEXT", nullable: true),
                QslVia = table.Column<string>(name: "qsl_via", type: "TEXT", nullable: true),
                DateTime = table.Column<string>(name: "date_time", type: "TEXT", nullable: true),
                DateTimeOff = table.Column<string>(name: "date_time_off", type: "TEXT", nullable: true),
                Qth = table.Column<string>(name: "qth", type: "TEXT", nullable: true),
                RstRcvd = table.Column<string>(name: "rst_rcvd", type: "TEXT", nullable: true),
                RstSent = table.Column<string>(name: "rst_sent", type: "TEXT", nullable: true),
                StationCallsign = table.Column<string>(name: "station_callsign", type: "TEXT", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QSOs", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "QSOs");
    }
}
