# Laporan ETS PBKK - Sistem Pencarian SPKLU

**Nama** : Liem, Alfred Haryanto  
**NRP** : 5025241100  
**Kelas** : PBKK (D)  

---

[Link Demo Video Youtube](https://youtu.be/kiIOYZ8FxGg)  
[Link Source Code GitHub](https://github.com/freddskii/ETS-PBKK/tree/main/sistem_pencarian_SPKLU)

---

## 1. Latar Belakang

Pada tugas ETS PBKK kali ini, saya dan tim membuat aplikasi sederhana bernama **SPKLU Finder (Sistem Pencarian SPKLU)** menggunakan C# .NET WPF. Aplikasi ini bertujuan untuk membantu pengguna kendaraan listrik (EV) dalam mencari dan melihat informasi stasiun pengisian kendaraan listrik umum (SPKLU) terdekat secara cepat dan mudah melalui visualisasi peta yang interaktif.

### Deskripsi Singkat Aplikasi
**SPKLU Finder** adalah aplikasi desktop berbasis C# .NET WPF yang dirancang untuk mempermudah pemilik kendaraan listrik (Electric Vehicle / EV) dalam mencari dan meninjau informasi Stasiun Pengisian Kendaraan Listrik Umum (SPKLU). Aplikasi ini menyajikan katalog data teknis stasiun secara lengkap—seperti nama stasiun, kota, alamat, tipe charger (AC/DC Fast), daya maksimum (kW), ketersediaan port, estimasi tarif per kWh, hingga status operasional terkini—yang dipadukan dengan peta digital interaktif (Leaflet.js & OpenStreetMap) serta kemudahan navigasi langsung ke Google Maps.

### Manfaat Aplikasi
1. **Mengurangi *Range Anxiety* Pengemudi EV:** Memberikan kepastian lokasi stasiun pengisian daya terdekat sehingga pengendara dapat bepergian tanpa rasa cemas kehabisan daya baterai.
2. **Efisiensi Pemilihan Charger:** Memudahkan pengguna memilih stasiun yang sesuai dengan spesifikasi kendaraan dan kebutuhan waktu, seperti pengisian cepat (*DC Fast Charging* 50–60 kW) atau *Ultra Fast Charging* (150–200 kW) di area rest area tol dan pusat kota.
3. **Kepastian Ketersediaan Unit:** Meminimalisir risiko mendatangi stasiun yang sedang tidak dapat digunakan berkat indikator status operasional yang jelas (*Tersedia*, *Terpakai*, atau *Maintenance*).
4. **Kemudahan Akses Navigasi:** Mempercepat penentuan rute perjalanan dengan fitur terintegrasi *Buka di Google Maps* serta tombol *Salin Koordinat GPS* ke clipboard sistem.
5. **Aplikasi Ringan & Bebas Biaya Layanan Peta:** Berjalan secara cepat dan responsif dengan penyimpanan data *in-memory* serta menggunakan mesin peta OpenStreetMap komunitas tanpa ketergantungan API Key berbayar.

## 2. Kebutuhan & Persyaratan

Ketentuan dan kebutuhan fitur yang diimplementasikan pada aplikasi ini meliputi:
- **Dashboard:** Menampilkan ringkasan total SPKLU, SPKLU yang tersedia, serta jumlah charger DC Fast dan DC Ultra Fast.
- **Pencarian Real-Time:** Pencarian berdasarkan nama stasiun, kota, atau alamat stasiun.
- **Filter Multi-Kriteria:** Filter berdasarkan tipe charger (AC, DC Fast, DC Ultra Fast) dan status operasional (Tersedia, Terpakai, Maintenance).
- **Tabel Data (DataGrid):** Menampilkan daftar SPKLU dengan badge status berwarna.
- **Panel Detail SPKLU:** Menampilkan daya (kW), jumlah port, tarif per kWh, alamat lengkap, dan koordinat.
- **Peta Interaktif:** Peta terintegrasi menggunakan Leaflet.js & OpenStreetMap di dalam WebView2 (gratis tanpa API key).
- **Aksi Navigasi:** Tombol untuk membuka rute di Google Maps dan tombol untuk menyalin koordinat ke clipboard.
- **Tombol Reset:** Mengembalikan pencarian dan filter ke kondisi semula.

## 3. Design Sistem

Sistem ini dirancang secara sederhana dan efisien menggunakan arsitektur *Code-Behind / Event-Driven*:
- **Data In-Memory:** Menggunakan data dummy SPKLU pada kelas `SpkluDataService` (12 lokasi di sekitar Jawa dan Bali) sehingga tidak memerlukan setup database terpisah.
- **Model Data (`SpkluLocation`):** Menyimpan atribut stasiun (Id, Nama, Kota, Alamat, Tipe Charger, Daya, Port, Status, Koordinat, dan helper warna badge).
- **Tata Letak UI (Layout 2 Panel):**
  - **Atas:** Header dan 4 Kartu Statistik metrik.
  - **Kiri:** Kolom pencarian, dropdown filter, dan tabel DataGrid.
  - **Kanan:** Peta interaktif WebView2 dan Panel Detail stasiun terpilih.
  - **Bawah:** Status bar penghitung data aktif.

### Struktur Folder

```text
sistem_pencarian_SPKLU/
├── Assets/
│   └── MapTemplate.html           # Template Leaflet.js & OpenStreetMap
├── Fonts/                         # Asset font Poppins untuk UI aplikasi
│   ├── Poppins-Regular.ttf
│   ├── Poppins-Medium.ttf
│   ├── Poppins-SemiBold.ttf
│   └── Poppins-Bold.ttf
├── Models/
│   └── SpkluLocation.cs           # Model data entitas SPKLU & properti visual
├── Services/
│   └── SpkluDataService.cs        # Penyedia data dummy in-memory
├── App.xaml / App.xaml.cs         # Resource global, palet warna, & startup
├── MainWindow.xaml                # Desain antarmuka XAML
├── MainWindow.xaml.cs             # Event handler, filter LINQ, & integrasi peta
└── sistem_pencarian_SPKLU.csproj  # Konfigurasi project .NET & dependency WebView2
```

## 4. Implementasi

Aplikasi diimplementasikan menggunakan beberapa teknologi dan komponen utama:
- **WPF & XAML:** Styling antarmuka modern dengan palet warna navy/slate, rounded corners, dan font *Poppins*.
- **Pencarian & Filter (LINQ):** Menggunakan query LINQ pada event `TextChanged` dan `SelectionChanged` di `MainWindow.xaml.cs` untuk memfilter data secara instan tanpa lag.
- **Peta Interaktif (WebView2 + Leaflet.js):** Peta dimuat menggunakan Leaflet.js dan OpenStreetMap. Ketika pengguna memilih baris SPKLU pada tabel, C# mengeksekusi script JavaScript `updateLocation()` via `ExecuteScriptAsync` sehingga kamera peta bergeser (*flyTo*) secara halus ke titik stasiun beserta popup informasinya.
- **Navigasi Google Maps & Clipboard:** Menggunakan `Process.Start` untuk membuka browser bawaan menuju Google Maps sesuai koordinat stasiun, serta `Clipboard.SetText` untuk menyalin koordinat.

### Cara Menjalankan Program

**Prasyarat:**
- Sistem Operasi: Windows 10 / 11
- .NET 8.0 SDK / .NET 10.0 SDK atau .NET Desktop Runtime
- Microsoft Edge WebView2 Runtime (bawaan Windows 10/11)

**Langkah Menjalankan:**
1. **Menggunakan Terminal (.NET CLI):**
   ```bash
   cd sistem_pencarian_SPKLU
   dotnet run
   ```
2. **Menggunakan Visual Studio 2022:**
   - Buka berkas `sistem_pencarian_SPKLU.csproj` di Visual Studio.
   - Tekan tombol **Start** atau tekan `F5` / `Ctrl + F5`.

## 5. Dokumentasi

Pengujian dilakukan untuk memastikan seluruh fitur berjalan dengan baik:
- **Load Awal:** Aplikasi berhasil memuat 12 data SPKLU, kartu statistik terhitung otomatis, dan peta fokus pada item pertama.

![](./Image/Tampilan%20Awal.png)

- **Uji Pencarian:** Mengetik kata kunci seperti `"Surabaya"` langsung menyaring tabel menjadi 3 baris dan statistik ikut menyesuaikan secara dinamis.

![](./Image/Pencarian%20Dengan%20Nama.png)

- **Uji Filter:** Memilih filter tipe `"DC Fast Charging"` dan status `"Tersedia"` berhasil menampilkan data yang sesuai kriteria.

Searching Operational
![](./Image/Filter%20Status%20Operational.png)

Searching Tipe Charger
![](./Image/Filter%20Tipe%20Charger.png)

Searching Dual Filter
![](./Image/Dual%20Filter.png)

- **Uji Klik Tabel & Peta:** Mengklik baris stasiun lain berhasil memperbarui panel detail dan menggeser pin peta ke lokasi yang dipilih.

Klik Tabel
![](./Image/Tampilan%20SPKLU.png)

Klik Peta
![](./Image/Buka%20di%20Map.png)


- **Uji Reset & Tombol Aksi:** Tombol reset berhasil mengosongkan filter, tombol Google Maps membuka peramban dengan koordinat akurat, dan tombol salin koordinat berhasil menyalin teks ke clipboard.

![](./Image/Reset%20Filter.png)