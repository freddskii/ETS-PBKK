using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using sistem_pencarian_SPKLU.Models;
using sistem_pencarian_SPKLU.Services;

namespace sistem_pencarian_SPKLU;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<SpkluLocation> _allLocations = new();
    private List<SpkluLocation> _filteredLocations = new();
    private SpkluLocation? _selectedLocation;
    private bool _isWebViewInitialized = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LoadInitialData();
        InitializeWebViewAsync();
    }

    private void LoadInitialData()
    {
        // 1. Ambil data dummy SPKLU dari service
        _allLocations = SpkluDataService.GetSpkluLocations();

        // 2. Perbarui ringkasan statistik
        UpdateStatistics();

        // 3. Terapkan filter awal
        ApplyFilter();

        // 4. Pilih baris pertama secara default
        if (_filteredLocations.Count > 0)
        {
            dgSpklu.SelectedIndex = 0;
        }
    }

    private async void InitializeWebViewAsync()
    {
        try
        {
            bdrMapFallback.Visibility = Visibility.Visible;
            txtMapFallback.Text = "Menginisialisasi kontrol peta WebView2...";

            await MapWebView.EnsureCoreWebView2Async();
            MapWebView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 SPKLUFinder/1.0";
            _isWebViewInitialized = true;
            bdrMapFallback.Visibility = Visibility.Collapsed;

            // Render peta untuk lokasi terpilih saat ini jika ada
            if (_selectedLocation != null)
            {
                UpdateMapView(_selectedLocation);
            }
        }
        catch (Exception ex)
        {
            _isWebViewInitialized = false;
            bdrMapFallback.Visibility = Visibility.Visible;
            txtMapFallback.Text = $"Peta interaktif offline / WebView2 runtime tidak tersedia: {ex.Message}";
        }
    }

    private void UpdateStatistics()
    {
        var targetList = (_filteredLocations != null && _filteredLocations.Count > 0) ? _filteredLocations : _allLocations;
        if (txtTotalSpklu != null) txtTotalSpklu.Text = (_filteredLocations?.Count ?? _allLocations.Count).ToString();
        if (txtTotalTersedia != null) txtTotalTersedia.Text = targetList.Count(x => x.Status.Equals("Tersedia", StringComparison.OrdinalIgnoreCase)).ToString();
        if (txtTotalFast != null) txtTotalFast.Text = targetList.Count(x => x.TipeCharger.Equals("DC Fast Charging", StringComparison.OrdinalIgnoreCase)).ToString();
        if (txtTotalUltraFast != null) txtTotalUltraFast.Text = targetList.Count(x => x.TipeCharger.Equals("DC Ultra Fast Charging", StringComparison.OrdinalIgnoreCase)).ToString();
    }

    private void ApplyFilter()
    {
        if (_allLocations == null || _allLocations.Count == 0) return;

        var keyword = txtSearch?.Text?.Trim() ?? string.Empty;
        var selectedType = (cmbTipeCharger?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Semua Tipe Charger";
        var selectedStatus = (cmbStatus?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Semua Status";

        var query = _allLocations.AsEnumerable();

        // Filter berdasarkan kata kunci pencarian (Nama, Kota, Alamat)
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x =>
                x.NamaStasiun.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                x.Kota.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                x.Alamat.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        // Filter berdasarkan tipe charger
        if (!string.IsNullOrEmpty(selectedType) && !selectedType.Equals("Semua Tipe Charger", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(x => x.TipeCharger.Equals(selectedType, StringComparison.OrdinalIgnoreCase));
        }

        // Filter berdasarkan status
        if (!string.IsNullOrEmpty(selectedStatus) && !selectedStatus.Equals("Semua Status", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(x => x.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase));
        }

        _filteredLocations = query.ToList();

        if (dgSpklu != null)
        {
            dgSpklu.ItemsSource = _filteredLocations;
        }

        if (txtRecordCount != null)
        {
            txtRecordCount.Text = $"Menampilkan {_filteredLocations.Count} dari {_allLocations.Count} SPKLU";
        }

        // Perbarui angka statistik agar menyesuaikan dengan hasil filter (TP-02)
        UpdateStatistics();

        // Jika item yang sedang dipilih sebelumnya tidak ada dalam hasil filter baru, pilih item pertama jika ada
        if (_filteredLocations.Count > 0)
        {
            if (_selectedLocation == null || !_filteredLocations.Any(x => x.Id == _selectedLocation.Id))
            {
                if (dgSpklu != null)
                {
                    dgSpklu.SelectedIndex = 0;
                }
            }
        }
        else
        {
            ClearDetailPanel();
        }
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (txtSearchPlaceholder != null)
        {
            txtSearchPlaceholder.Visibility = string.IsNullOrEmpty(txtSearch.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        ApplyFilter();
    }

    private void CmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFilter();
    }

    private void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        if (txtSearch != null) txtSearch.Text = string.Empty;
        if (cmbTipeCharger != null) cmbTipeCharger.SelectedIndex = 0;
        if (cmbStatus != null) cmbStatus.SelectedIndex = 0;

        ApplyFilter();

        if (_filteredLocations.Count > 0)
        {
            dgSpklu.SelectedIndex = 0;
        }
    }

    private void DgSpklu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgSpklu.SelectedItem is SpkluLocation selected)
        {
            _selectedLocation = selected;
            UpdateDetailPanel(selected);
            UpdateMapView(selected);
        }
    }

    private void UpdateDetailPanel(SpkluLocation location)
    {
        txtDetailNama.Text = location.NamaStasiun;
        txtDetailAlamat.Text = location.Alamat;
        txtDetailKota.Text = location.Kota;
        txtDetailTipe.Text = location.TipeCharger;
        txtDetailDaya.Text = location.DayaDisplay;
        txtDetailPort.Text = location.PortDisplay;
        txtDetailTarif.Text = location.TarifPerKwh;
        txtDetailKoordinat.Text = location.KoordinatDisplay;
        txtMapCoordBadge.Text = location.KoordinatDisplay;

        txtDetailStatus.Text = location.Status;
        try
        {
            var brushConverter = new BrushConverter();
            if (brushConverter.ConvertFromString(location.StatusBgColor) is Brush bgBrush)
            {
                bdrDetailStatus.Background = bgBrush;
            }
            if (brushConverter.ConvertFromString(location.StatusTextColor) is Brush fgBrush)
            {
                txtDetailStatus.Foreground = fgBrush;
            }
        }
        catch
        {
            // Fallback default colors
        }

        btnGoogleMaps.IsEnabled = true;
        btnCopyCoords.IsEnabled = true;
    }

    private void ClearDetailPanel()
    {
        _selectedLocation = null;
        txtDetailNama.Text = "Tidak ada SPKLU terpilih";
        txtDetailAlamat.Text = "Silakan ubah filter atau kata kunci pencarian Anda.";
        txtDetailKota.Text = "-";
        txtDetailTipe.Text = "-";
        txtDetailDaya.Text = "-";
        txtDetailPort.Text = "-";
        txtDetailTarif.Text = "-";
        txtDetailKoordinat.Text = "-";
        txtMapCoordBadge.Text = "-";
        txtDetailStatus.Text = "-";
        btnGoogleMaps.IsEnabled = false;
        btnCopyCoords.IsEnabled = false;
    }

    private void UpdateMapView(SpkluLocation location)
    {
        if (!_isWebViewInitialized)
        {
            return;
        }

        try
        {
            var html = GenerateLeafletHtml(location);
            MapWebView.NavigateToString(html);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Gagal memuat peta WebView2: {ex.Message}");
        }
    }

    private string GenerateLeafletHtml(SpkluLocation loc)
    {
        var latStr = loc.Latitude.ToString(CultureInfo.InvariantCulture);
        var lngStr = loc.Longitude.ToString(CultureInfo.InvariantCulture);
        var safeName = loc.NamaStasiun.Replace("'", "\\'").Replace("\"", "\\\"");
        var safeAlamat = loc.Alamat.Replace("'", "\\'").Replace("\"", "\\\"");
        var safeTipe = loc.TipeCharger.Replace("'", "\\'").Replace("\"", "\\\"");
        var safeStatus = loc.Status.Replace("'", "\\'").Replace("\"", "\\\"");

        var badgeClass = loc.Status switch
        {
            "Tersedia" => "badge-tersedia",
            "Terpakai" => "badge-terpakai",
            "Maintenance" => "badge-maintenance",
            _ => "badge-tersedia"
        };

        return $@"<!DOCTYPE html>
<html lang=""id"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"" />
    <script src=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.js""></script>
    <style>
        html, body {{ height: 100%; width: 100%; margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; background-color: #0f172a; }}
        #map {{ height: 100%; width: 100%; }}
        .custom-popup .leaflet-popup-content-wrapper {{
            background: #ffffff;
            color: #1e293b;
            border-radius: 10px;
            padding: 4px;
            box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
        }}
        .popup-title {{ font-weight: 700; font-size: 13px; color: #0f172a; margin-bottom: 4px; }}
        .popup-badge {{ display: inline-block; font-size: 11px; font-weight: 600; padding: 2px 8px; border-radius: 9999px; margin-bottom: 6px; }}
        .badge-tersedia {{ background-color: #d1fae5; color: #065f46; }}
        .badge-terpakai {{ background-color: #fef3c7; color: #92400e; }}
        .badge-maintenance {{ background-color: #fee2e2; color: #991b1b; }}
        .popup-text {{ font-size: 11px; color: #475569; line-height: 1.4; }}
    </style>
</head>
<body>
    <div id=""map""></div>
    <script>
        try {{
            var map = L.map('map', {{ zoomControl: true }}).setView([{latStr}, {lngStr}], 15);
            var cartoVoyager = L.tileLayer('https://{{s}}.basemaps.cartocdn.com/rastertiles/voyager/{{z}}/{{x}}/{{y}}{{r}}.png', {{
                maxZoom: 19,
                subdomains: 'abcd',
                attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors &copy; <a href=""https://carto.com/attributions"">CARTO</a>'
            }});

            var esriMap = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{{z}}/{{y}}/{{x}}', {{
                maxZoom: 19,
                attribution: 'Tiles &copy; Esri'
            }});

            var cartoLight = L.tileLayer('https://{{s}}.basemaps.cartocdn.com/light_all/{{z}}/{{x}}/{{y}}{{r}}.png', {{
                maxZoom: 19,
                subdomains: 'abcd',
                attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors &copy; <a href=""https://carto.com/attributions"">CARTO</a>'
            }});

            cartoVoyager.addTo(map);

            var baseMaps = {{
                ""Peta Standar (OSM)"": cartoVoyager,
                ""Peta Terang"": cartoLight,
                ""Peta Jalan (Esri)"": esriMap
            }};
            L.control.layers(baseMaps).addTo(map);

            var popupContent = '<div class=""custom-popup"">' +
                '<div class=""popup-title"">{safeName}</div>' +
                '<span class=""popup-badge {badgeClass}"">{safeStatus}</span>' +
                '<div class=""popup-text"">' +
                '<strong>Tipe:</strong> {safeTipe}<br/>' +
                '<strong>Alamat:</strong> {safeAlamat}' +
                '</div></div>';

            L.marker([{latStr}, {lngStr}]).addTo(map)
                .bindPopup(popupContent)
                .openPopup();
        }} catch(e) {{
            document.body.innerHTML = '<div style=""color:white;padding:20px;font-family:sans-serif;"">Gagal memuat peta: ' + e.message + '</div>';
        }}
    </script>
</body>
</html>";
    }

    private void BtnGoogleMaps_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedLocation == null)
        {
            MessageBox.Show("Pilih SPKLU terlebih dahulu.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var lat = _selectedLocation.Latitude.ToString(CultureInfo.InvariantCulture);
        var lng = _selectedLocation.Longitude.ToString(CultureInfo.InvariantCulture);
        var url = $"https://www.google.com/maps/search/?api=1&query={lat},{lng}";

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Tidak dapat membuka Google Maps: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCopyCoords_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedLocation == null) return;

        try
        {
            var coords = $"{_selectedLocation.Latitude.ToString(CultureInfo.InvariantCulture)}, {_selectedLocation.Longitude.ToString(CultureInfo.InvariantCulture)}";
            Clipboard.SetText(coords);
            MessageBox.Show($"Koordinat berhasil disalin:\n{coords}", "Salin Koordinat", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Gagal menyalin koordinat: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}