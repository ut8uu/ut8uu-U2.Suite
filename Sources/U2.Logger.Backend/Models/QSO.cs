namespace U2.Logger.Backend.Models;

public class QSO
{
    public int Id { get; set; } // Primary key
    public string Callsign { get; set; }
    public string Snt { get; set; }
    public string Rcvd { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public string Band { get; set; }
    public string Mode { get; set; }
    public string Propagation { get; set; }
    public DateTime DateTime { get; set; }
}
