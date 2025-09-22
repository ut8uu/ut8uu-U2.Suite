using U2.Logger.Core.Models;

namespace U2.Logger.Backend.Services;

/// <summary>
/// A static class containing validation logic for QSO objects.
/// </summary>
public static class QSOValidator
{
    private static readonly HashSet<string> KnownModes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "SSB", "CW", "FM", "AM", "RTTY", "FT8", "PSK31", "SSTV", "JT65", "JS8", "DMR", "D-STAR", "C4FM"
    };

    private static readonly Dictionary<string, (double min, double max)> KnownBands = new Dictionary<string, (double, double)>(StringComparer.OrdinalIgnoreCase)
    {
        { "160m", (1.8, 2.0) },
        { "80m", (3.5, 4.0) },
        { "40m", (7.0, 7.3) },
        { "30m", (10.1, 10.15) },
        { "20m", (14.0, 14.35) },
        { "17m", (18.068, 18.168) },
        { "15m", (21.0, 21.45) },
        { "12m", (24.89, 24.99) },
        { "10m", (28.0, 29.7) },
        { "6m", (50.0, 54.0) },
        { "2m", (144.0, 148.0) },
        { "70cm", (430.0, 450.0) }
    };

    /// <summary>
    /// Validates a QSO DTO object against a set of business rules.
    /// </summary>
    /// <param name="qso">The QSO object to validate.</param>
    /// <param name="message">An output parameter for the validation error message.</param>
    /// <returns>True if the QSO is valid; otherwise, false.</returns>
    public static bool IsValid(Core.Models.QSO qso, out string message)
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

        // Rule 2: DateTime is mandatory.
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
}
