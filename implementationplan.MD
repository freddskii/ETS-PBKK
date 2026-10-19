# Rencana Implementasi: Sistem Pencarian SPKLU & Maps Sederhana

## 1. Ringkasan & Cakupan Aplikasi

Sistem Pencarian SPKLU (Stasiun Pengisian Kendaraan Listrik Umum) adalah aplikasi desktop yang mempermudah pengguna mencari titik pengisian daya kendaraan listrik terdekat.

* **Sifat Data:** In-Memory (Hardcoded List / JSON file), tidak membutuhkan setup SQL Server/database.
* **Arsitektur:** Event-Driven / Code-Behind Sederhana (tanpa ketat menggunakan MVVM agar penulisan kode lebih singkat dan efisien untuk tugas kuliah).
* **Komponen Utama:** Header Dashboard, Kartu Ringkasan Ringkas, Filter & Pencarian, DataGrid Informasi, Panel Detail, dan Embedded Map View.

---

## 2. Teknologi & Library

| Layar / Fungsi | Teknologi | Keterangan |
| --- | --- | --- |
| **Bahasa & Framework** | C# / .NET 8 WPF | Memberikan performa tinggi dan kustomisasi Rich UI. |
| **UI Markup** | XAML | Styling visual, Layout Grid/StackPanel, Rounded Border, dan Palette Warna Modern. |
| **Peta (Maps)** | `Microsoft.Web.WebView2` | Control browser bawaan WPF untuk merender OpenStreetMap / Leaflet.js secara gratis tanpa API Key berbayar. |
| **Data Provider** | In-Memory `List<SpkluLocation>` | Data dummy SPKLU disimpan dalam memori aplikasi. |
| **IDE** | Visual Studio 2022 | Environment standar pengembangan WPF. |

---

## 3. Fitur Utama Aplikasi

1. **Dashboard Ringkasan (Statistic Cards):**
* Menampilkan jumlah total SPKLU terdaftar.
* Jumlah SPKLU dengan status *Tersedia (Available)* vs *Terpakai (In Use)*.
* Jumlah tipe charger: AC Charging, DC Fast Charging, dan Ultra Fast Charging.


2. **Fitur Pencarian & Filter Multi-Kriteria:**
* **Search Bar:** Pencarian berdasarkan Nama SPKLU, Kota, atau Lokasi/Alamat.
* **Filter Combo Box:** Filter berdasarkan Tipe Charger (AC/DC) atau Status Operasional.


3. **Daftar SPKLU (Data Table):**
* Menampilkan ringkasan data SPKLU dalam bentuk `DataGrid` atau `ListView` yang rapi.
* Fitur klik/pilih baris untuk melihat detail dan memperbarui koordinat peta.


4. **Peta Interaktif (Interactive Embedded Map):**
* Menampilkan peta visual yang berpusat pada koordinat SPKLU yang dipilih (`Latitude` & `Longitude`).
* Menampilkan marker/pin posisi stasiun pengisian daya.


5. **Panel Detail SPKLU:**
* Informasi daya (kW), jumlah port charging, tarif per kWh, dan alamat lengkap.
* Tombol *“Buka di Google Maps”* (membuka browser eksternal untuk navigasi rute rincian).



---

## 4. Struktur Project

```text
SpkluFinderApp/
│
├── App.xaml / App.xaml.cs               # Entry point aplikasi & Global Resource Style
├── MainWindow.xaml                       # Tampilan utama (XAML Rich UI)
├── MainWindow.xaml.cs                    # Event handler logic (Search, Selection, Map Update)
│
├── Models/
│   └── SpkluLocation.cs                 # Blueprint data SPKLU (Properties)
│
├── Services/
│   └── SpkluDataService.cs              # Provider data dummy (Return List<SpkluLocation>)
│
└── Assets/
    └── MapTemplate.html                 # (Opsional) Template Leaflet.js untuk render peta di WebView2

```

---

## 5. Model Data (*In-Memory*)

Model data `SpkluLocation` menyimpan atribut penting dari stasiun SPKLU:

* **Id:** Identifier numerik.
* **NamaStasiun:** Nama penyedia / lokasi (misal: *SPKLU PLN UID Jawa Timur*).
* **Kota:** Nama kota / wilayah.
* **Alamat:** Alamat jalan rinci.
* **TipeCharger:** Tipe arus (*AC Fast Charging*, *DC Fast Charging*, *DC Ultra Fast Charging*).
* **DayaMaxKW:** Kapasitas daya maksimum (misal: `22 kW`, `50 kW`, `150 kW`).
* **JumlahPort:** Total port pengisian daya.
* **Status:** Operational Status (*Tersedia*, *Penuh*, *Maintenance*).
* **Latitude:** Koordinat Lintang.
* **Longitude:** Koordinat Bujur.

---

## 6. Layout & UI Mapping (XAML Structure)

Tampilan `MainWindow.xaml` dibagi menjadi 3 area utama menggunakan **Grid Definitions**:

```text
+-----------------------------------------------------------------------------------+
|  HEADER PANEL: Judul Aplikasi "SPKLU Finder & Interactive Map"                    |
+-----------------------------------------------------------------------------------+
|  STATISTIC CARDS: [Total SPKLU]  [Tersedia]  [Fast Charging]  [Ultra Fast]        |
+--------------------------------------------------+--------------------------------+
|  LEFT PANEL (Daftar & Filter)                    | RIGHT PANEL (Map & Detail)     |
|  ----------------------------------------------  | ------------------------------ |
|  [ Search Box ]  [ Filter Combo ]  [ Reset ]     | [ WebView2 Map View ]          |
|                                                  | (Menampilkan titik koordinat)  |
|  +--------------------------------------------+  |                                |
|  | DataGrid / ListView                        |  | ------------------------------ |
|  | - Nama SPKLU                               |  | [ Panel Detail SPKLU ]         |
|  | - Kota                                     |  | - Alamat, Tarif, Daya kW       |
|  | - Tipe Charger                             |  | - Status Port                  |
|  | - Status                                   |  | - Button: Buka Google Maps     |
|  +--------------------------------------------+  |                                |
+--------------------------------------------------+--------------------------------+

```

---

## 7. Alur Kerja Aplikasi (Workflow Logic)

```text
               +----------------------------------+
               |        Aplikasi Dijalankan        |
               +----------------------------------+
                                |
                                v
               +----------------------------------+
               | Load Data Mock dari DataService  |
               +----------------------------------+
                                |
                                v
               +----------------------------------+
               | Refresh DataGrid & Statistics    |
               | Render WebView2 Peta Awal        |
               +----------------------------------+
                                |
        +-----------------------+-----------------------+
        |                                               |
        v                                               v
[ User mengetik Search / Filter ]            [ User Mengklik Baris DataGrid ]
        |                                               |
        v                                               v
Filter List<SpkluLocation>                   Ambil item SpkluLocation
        |                                               |
        v                                               v
Update Tampilan DataGrid                     1. Update Panel Detail
Update Kartu Statistik                       2. Kirim Lat/Lng ke WebView2
                                             3. Peta berpindah & fokus ke lokasi

```

---

## 8. Pemetaan Komponen UI ke Event Handler

| Komponen UI | Jenis Kontrol | Event / Trigger | Aksi Event Handler |
| --- | --- | --- | --- |
| **Search Box** | `TextBox` | `TextChanged` | Memfilter daftar SPKLU secara *real-time* berdasarkan kata kunci nama/kota. |
| **Filter Tipe** | `ComboBox` | `SelectionChanged` | Memfilter daftar SPKLU berdasarkan tipe charger yang dipilih. |
| **Tabel Data** | `DataGrid` | `SelectionChanged` | Mengambil data baris aktif, memperbarui Panel Detail, dan memperbarui URL/koordinat peta di WebView2. |
| **Tombol Reset** | `Button` | `Click` | Mengosongkan Search Box, mengembalikan filter ke "Semua", dan merefresh DataGrid. |
| **Tombol Navigasi** | `Button` | `Click` | Membuka koordinat SPKLU terpilih di Google Maps via browser default OS. |

---

## 9. Skenario Pengujian (Test Plan)

| ID Test | Fitur | Langkah Pengujian | Hasil yang Diharapkan |
| --- | --- | --- | --- |
| **TP-01** | **Initial Load** | Buka aplikasi. | Seluruh data dummy SPKLU muncul di DataGrid, angka statistik terhitung tepat, dan peta berhasil dimuat. |
| **TP-02** | **Search** | Ketik kata kunci (misal: "Surabaya") pada Search Box. | DataGrid hanya menampilkan SPKLU yang berada di Surabaya. Angka statistik menyesuaikan. |
| **TP-03** | **Filter** | Pilih "DC Fast Charging" pada ComboBox Filter Tipe. | DataGrid memfilter hanya stasiun dengan spesifikasi DC Fast Charging. |
| **TP-04** | **Map Interaction** | Klik salah satu baris SPKLU pada DataGrid. | WebView2 merender ulang peta dan menggeser pusat tampilan peta ke titik koordinat SPKLU tersebut. |
| **TP-05** | **Detail View** | Klik salah satu baris SPKLU. | Panel detail menampilkan informasi lengkap (daya kW, alamat, status port). |
| **TP-06** | **External Map** | Klik tombol "Buka di Google Maps". | Web browser bawaan komputer terbuka dan mengarahkan ke Google Maps dengan pin lokasi SPKLU. |

---

## 10. Tahapan Pengerjaan Tugas (Student Roadmap)

1. **Tahap 1: Setup Project**
* Buat Project WPF Application (.NET 8) di Visual Studio.
* Install NuGet Package `Microsoft.Web.WebView2`.


2. **Tahap 2: Pembuatan Model & Mock Data**
* Buat kelas model `SpkluLocation.cs`.
* Buat kelas `SpkluDataService.cs` yang mengembalikan 8–12 data dummy SPKLU lengkap dengan koordinat real (misal: area Jakarta/Surabaya/Bandung).


3. **Tahap 3: Desain UI XAML Modern**
* Buat layout utama menggunakan `Grid`, `Border` ber-corner radius, dan susunan `StackPanel`.
* Atur skema warna modern (contoh: Header Biru Gelap `#1E293B`, background netral `#F8FAFC`, dan aksen Hijau `#10B981` untuk representasi EV/Listrik).


4. **Tahap 4: Pengkodean Logika (Code-Behind)**
* Hubungkan `ItemsSource` DataGrid dengan data dari `SpkluDataService`.
* Buat fungsi hitung statistik sederhana (*Count*, *Where*) untuk memperbarui label kartu ringkasan.
* Implementasikan logika pencarian dan pemfilteran pada event `TextChanged` dan `SelectionChanged`.


5. **Tahap 5: Integrasi Peta (WebView2)**
* Inisialisasi control `WebView2`.
* Buat fungsi helper untuk meng-generate URL OpenStreetMap Embed berdasarkan `Latitude` dan `Longitude` dari SPKLU terpilih, lalu panggil `WebView2.CoreWebView2.Navigate()`.


6. **Tahap 6: Pengujian & Finishing**
* Jalankan skenario pengujian (TP-01 hingga TP-06).
* Rapikan margin, padding, font, dan animasi UI ringan.