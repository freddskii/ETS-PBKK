namespace sistem_pencarian_SPKLU.Models;

public class SpkluLocation
{
    public int Id { get; set; }
    public string NamaStasiun { get; set; } = string.Empty;
    public string Kota { get; set; } = string.Empty;
    public string Alamat { get; set; } = string.Empty;
    public string TipeCharger { get; set; } = string.Empty;
    public int DayaMaxKW { get; set; }
    public int JumlahPort { get; set; }
    public string Status { get; set; } = "Tersedia";
    public string TarifPerKwh { get; set; } = "Rp 2.466 / kWh";
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Helper properties for easy data binding
    public string DayaDisplay => $"{DayaMaxKW} kW";
    public string PortDisplay => $"{JumlahPort} Port";
    public string KoordinatDisplay => $"{Latitude:F4}, {Longitude:F4}";

    public string StatusBgColor => Status switch
    {
        "Tersedia" => "#D1FAE5",      // Emerald light
        "Terpakai" => "#FEF3C7",      // Amber light
        "Maintenance" => "#FEE2E2",   // Red light
        _ => "#F1F5F9"
    };

    public string StatusTextColor => Status switch
    {
        "Tersedia" => "#065F46",      // Emerald dark
        "Terpakai" => "#92400E",      // Amber dark
        "Maintenance" => "#991B1B",   // Red dark
        _ => "#334155"
    };
}
