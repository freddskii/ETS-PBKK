# Dokumentasi Aplikasi SPKLU Finder

Aplikasi desktop berbasis **WPF (.NET 8)** untuk membantu pengguna kendaraan listrik menemukan lokasi Stasiun Pengisian Kendaraan Listrik Umum (SPKLU), memantau ketersediaan port, spesifikasi daya, serta navigasi lokasi berbasis peta interaktif.

---

## 1. Latar Belakang & Manfaat

* **Latar Belakang:** Tingginya pertumbuhan Kendaraan Listrik (EV) membutuhkan kepastian lokasi pengisian daya, kepastian jenis colokan/daya (AC vs DC Fast Charging), dan ketersediaan port tanpa harus menggunakan aplikasi berbasis server/database yang berat.
* **Manfaat:** Mempermudah pengemudi EV mencari lokasi pengisian terdekat, mengecek status stasiun secara real-time (*Tersedia/Terpakai/Maintenance*), dan mendapatkan rute navigasi cepat ke Google Maps.

---

## 2. Fitur Utama & Dokumentasi Antarmuka

Berikut adalah rincian fitur aplikasi beserta penempatan screenshot dokumentasinya:

### A. Dashboard Ringkasan & Status System
Menampilkan indikator status data *In-Memory* beserta 4 kartu statistik utama: **Total SPKLU**, **SPKLU Tersedia**, **DC Fast Charger**, dan **Ultra Fast Charger**.

<img width="1917" height="1078" alt="image" src="https://github.com/user-attachments/assets/4f607735-c98c-4747-bd97-b12b920bb216" />

---

### B. Pencarian & Filter Multi-Kriteria
Pengguna dapat mencari stasiun pengisian secara *real-time* berdasarkan kata kunci nama/kota/alamat, serta memfilter berdasarkan **Tipe Charger** dan **Status Operasional**.

<img width="975" height="722" alt="image" src="https://github.com/user-attachments/assets/81ac9fa2-e6a8-48b5-857e-409fe36d7ca6" />

---

### C. Tabel Data SPKLU (Interactive DataGrid)
Daftar stasiun disajikan rapi dengan penanda status berbobot warna (*Hijau = Tersedia*, *Kuning = Terpakai*, *Merah = Maintenance*). Memilih salah satu baris akan memperbarui informasi detail dan peta.

<img width="977" height="717" alt="image" src="https://github.com/user-attachments/assets/f7bef519-8d30-4fde-a14d-8c382b35404d" />

---

### D. Peta Interaktif & Panel Detail (WebView2 + Leaflet.js)
Integrasi peta OpenStreetMap yang menggeser titik *marker/pin* sesuai dengan SPKLU yang dipilih, dilengkapi *popup* informasi dan panel detail spesifikasi teknis di bagian kanan bawah.

<img width="1876" height="721" alt="image" src="https://github.com/user-attachments/assets/f9afc7e0-7fcf-4b35-a0eb-2f3c6ff55f2d" />

---

### E. Aksi Navigasi & Salin Koordinat
Menyediakan tombol **"Buka di Google Maps"** yang akan mengarahkan lokasi stasiun langsung ke web browser eksternal, serta tombol **"Salin Koordinat"** untuk menyalin koordinat latitude/longitude ke *clipboard*.

<img width="887" height="337" alt="image" src="https://github.com/user-attachments/assets/926ad73e-0eb8-40a1-abfb-adefb490c745" />

<img width="1901" height="920" alt="image" src="https://github.com/user-attachments/assets/f5e3c1ca-1bca-4389-b40c-81b055264978" />

---

## 3. Arsitektur & Teknologi

* **Framework & UI:** C# / .NET 8 WPF (Rich UI dengan `Grid`, `StackPanel`, dan `Border` kustom).
* **Komponen Peta:** `Microsoft.Web.WebView2` (Rendering OpenStreetMap & Leaflet.js tanpa API Key).
* **Manajemen Data:** In-Memory Data Collection (`List<SpkluLocation>`).
* **Arsitektur:** Event-Driven / Code-Behind Sederhana.

---

## 4. Struktur Proyek

```text
SpkluFinderApp/
├── App.xaml / App.xaml.cs          # Entry point & styling global
├── MainWindow.xaml                  # Layout XAML Rich UI
├── MainWindow.xaml.cs               # Event Handler (Search, Filter, Map Navigate)
├── Models/
│   └── SpkluLocation.cs            # Blueprint entitas data SPKLU
├── Services/
│   └── SpkluDataService.cs         # Provider dummy data SPKLU
└── screenshots/                    # Folder penyimpanan gambar dokumentasi
```

---

## 5. Ringkasan Skenario Pengujian

1. **Initial Load:** Memastikan seluruh data dummy (12 stasiun), angka statistik, dan peta awal berhasil dirender.
2. **Pencarian Real-Time:** Memastikan tabel hanya menampilkan data yang sesuai dengan kata kunci pada Search Box.
3. **Filter Multi-Kriteria:** Memastikan pemfilteran gabungan Tipe Charger dan Status berfungsi tepat.
4. **Interaksi Peta:** Memastikan klik baris tabel berhasil menggeser koordinat pusat peta pada kontrol WebView2.
5. **Navigasi Eksternal:** Memastikan tombol "Buka di Google Maps" membuka browser default dengan koordinat persis.
