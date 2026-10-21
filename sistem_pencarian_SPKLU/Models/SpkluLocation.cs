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
    public string PortDisplay => $"{JumlahPort} port";
    public string KoordinatDisplay => $"{Latitude:F4}, {Longitude:F4}";

    // Label status yang ditampilkan di UI
    public string StatusLabel => Status switch
    {
        "Maintenance" => "Dalam perbaikan",
        _ => Status
    };

    public string StatusBgColor => Status switch
    {
        "Tersedia" => "#D1FAE5",      // Hijau muda
        "Terpakai" => "#FEF3C7",      // Kuning muda
        "Maintenance" => "#FEE2E2",   // Merah muda
        _ => "#E9EFF4"
    };

    public string StatusTextColor => Status switch
    {
        "Tersedia" => "#065F46",      // Hijau tua
        "Terpakai" => "#92400E",      // Kuning tua
        "Maintenance" => "#991B1B",   // Merah tua
        _ => "#1E2A3A"
    };
}
