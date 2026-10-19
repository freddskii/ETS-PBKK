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
    private bool _isMapReady = false;

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

            MapWebView.NavigationCompleted += (s, e) =>
            {
                _isMapReady = true;
                bdrMapFallback.Visibility = Visibility.Collapsed;
            };

            await MapWebView.EnsureCoreWebView2Async();
            MapWebView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 SPKLUFinder/1.0";
            _isWebViewInitialized = true;

            // Render peta awal
            var initialLoc = _selectedLocation ?? (_allLocations.Count > 0 ? _allLocations[0] : null);
            if (initialLoc != null)
            {
                MapWebView.NavigateToString(GenerateLeafletHtml(initialLoc));
            }
            else
            {
                bdrMapFallback.Visibility = Visibility.Collapsed;
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
        if (!_isWebViewInitialized || MapWebView == null)
        {
            return;
        }

        try
        {
            if (_isMapReady)
            {
                var lat = location.Latitude.ToString(CultureInfo.InvariantCulture);
                var lng = location.Longitude.ToString(CultureInfo.InvariantCulture);
                var name = EscapeJs(location.NamaStasiun);
                var addr = EscapeJs(location.Alamat);
                var tipe = EscapeJs(location.TipeCharger);
                var status = EscapeJs(location.Status);

                _ = MapWebView.ExecuteScriptAsync($"updateLocation({lat}, {lng}, '{name}', '{addr}', '{tipe}', '{status}');");
            }
            else
            {
                var html = GenerateLeafletHtml(location);
                MapWebView.NavigateToString(html);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Gagal memuat peta WebView2: {ex.Message}");
        }
    }

    private static string EscapeJs(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Replace("\\", "\\\\")
                    .Replace("'", "\\'")
                    .Replace("\"", "\\\"")
                    .Replace("\r", "")
                    .Replace("\n", " ");
    }

    private string GenerateLeafletHtml(SpkluLocation loc)
    {
        var latStr = loc.Latitude.ToString(CultureInfo.InvariantCulture);
        var lngStr = loc.Longitude.ToString(CultureInfo.InvariantCulture);
        var safeName = EscapeJs(loc.NamaStasiun);
        var safeAlamat = EscapeJs(loc.Alamat);
        var safeTipe = EscapeJs(loc.TipeCharger);
        var safeStatus = EscapeJs(loc.Status);

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
            box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.15);
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
        var map;
        var marker;

        function initMap() {{
            map = L.map('map', {{ zoomControl: true }}).setView([{latStr}, {lngStr}], 15);
            L.tileLayer('https://tile.openstreetmap.de/{{z}}/{{x}}/{{y}}.png', {{
                maxZoom: 19,
                attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors'
            }}).addTo(map);

            updateLocation({latStr}, {lngStr}, '{safeName}', '{safeAlamat}', '{safeTipe}', '{safeStatus}');
        }}

        function updateLocation(lat, lng, name, address, chargerType, status) {{
            if (!map) return;
            map.flyTo([lat, lng], 16, {{ animate: true, duration: 0.8 }});
            if (marker) {{
                map.removeLayer(marker);
            }}

            var badgeClass = status === 'Terpakai' ? 'badge-terpakai' : (status === 'Maintenance' ? 'badge-maintenance' : 'badge-tersedia');
            var popupContent = '<div class=""custom-popup"">' +
                '<div class=""popup-title"">' + (name || 'SPKLU') + '</div>' +
                '<span class=""popup-badge ' + badgeClass + '"">' + (status || 'Tersedia') + '</span>' +
                '<div class=""popup-text"">' +
                '<strong>Tipe:</strong> ' + (chargerType || '-') + '<br/>' +
                '<strong>Alamat:</strong> ' + (address || '-') +
                '</div></div>';

            marker = L.marker([lat, lng]).addTo(map).bindPopup(popupContent).openPopup();
        }}

        window.addEventListener('DOMContentLoaded', initMap);
    </script>
</body>
</html>";
    }

    public static string GenerateOsmEmbedUrl(SpkluLocation loc)
    {
        // Bounding box offset ~500m di sekitar titik koordinat SPKLU
        const double delta = 0.005;
        var minLng = (loc.Longitude - delta).ToString(CultureInfo.InvariantCulture);
        var minLat = (loc.Latitude - delta).ToString(CultureInfo.InvariantCulture);
        var maxLng = (loc.Longitude + delta).ToString(CultureInfo.InvariantCulture);
        var maxLat = (loc.Latitude + delta).ToString(CultureInfo.InvariantCulture);
        var markerLat = loc.Latitude.ToString(CultureInfo.InvariantCulture);
        var markerLng = loc.Longitude.ToString(CultureInfo.InvariantCulture);

        return $"https://www.openstreetmap.org/export/embed.html?bbox={minLng}%2C{minLat}%2C{maxLng}%2C{maxLat}&layer=mapnik&marker={markerLat}%2C{markerLng}";
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