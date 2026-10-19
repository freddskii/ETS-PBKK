using System.Collections.Generic;
using sistem_pencarian_SPKLU.Models;

namespace sistem_pencarian_SPKLU.Services;

public class SpkluDataService
{
    public static List<SpkluLocation> GetSpkluLocations()
    {
        return new List<SpkluLocation>
        {
            new SpkluLocation
            {
                Id = 1,
                NamaStasiun = "SPKLU PLN UID Jawa Timur",
                Kota = "Surabaya",
                Alamat = "Jl. Embong Trengguli No. 19-21, Genteng, Surabaya",
                TipeCharger = "DC Fast Charging",
                DayaMaxKW = 50,
                JumlahPort = 2,
                Status = "Tersedia",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -7.2654,
                Longitude = 112.7482
            },
            new SpkluLocation
            {
                Id = 2,
                NamaStasiun = "SPKLU Grand City Mall Surabaya",
                Kota = "Surabaya",
                Alamat = "Jl. Walikota Mustajab No. 1, Ketabang, Genteng, Surabaya",
                TipeCharger = "AC Fast Charging",
                DayaMaxKW = 22,
                JumlahPort = 4,
                Status = "Tersedia",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -7.2625,
                Longitude = 112.7508
            },
            new SpkluLocation
            {
                Id = 3,
                NamaStasiun = "SPKLU Rest Area KM 575A Tol Solo-Ngawi",
                Kota = "Ngawi",
                Alamat = "Rest Area KM 575A, Tol Solo-Ngawi, Ngawi, Jawa Timur",
                TipeCharger = "DC Ultra Fast Charging",
                DayaMaxKW = 200,
                JumlahPort = 4,
                Status = "Terpakai",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -7.3912,
                Longitude = 111.3598
            },
            new SpkluLocation
            {
                Id = 4,
                NamaStasiun = "SPKLU PLN Kantor Pusat Gambir",
                Kota = "Jakarta Pusat",
                Alamat = "Jl. Medan Merdeka Timur No. 3, Gambir, Jakarta Pusat",
                TipeCharger = "DC Ultra Fast Charging",
                DayaMaxKW = 150,
                JumlahPort = 4,
                Status = "Tersedia",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -6.1764,
                Longitude = 106.8315
            },
            new SpkluLocation
            {
                Id = 5,
                NamaStasiun = "SPKLU Senayan Park (Spark)",
                Kota = "Jakarta Pusat",
                Alamat = "Jl. Gerbang Pemuda No. 3, Gelora, Tanah Abang, Jakarta Pusat",
                TipeCharger = "DC Fast Charging",
                DayaMaxKW = 50,
                JumlahPort = 2,
                Status = "Terpakai",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -6.2132,
                Longitude = 106.8005
            },
            new SpkluLocation
            {
                Id = 6,
                NamaStasiun = "SPKLU Gedung Sate",
                Kota = "Bandung",
                Alamat = "Jl. Diponegoro No. 22, Citarum, Bandung Wetan, Bandung",
                TipeCharger = "DC Fast Charging",
                DayaMaxKW = 50,
                JumlahPort = 2,
                Status = "Tersedia",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -6.9025,
                Longitude = 107.6186
            },
            new SpkluLocation
            {
                Id = 7,
                NamaStasiun = "SPKLU Paris Van Java (PVJ)",
                Kota = "Bandung",
                Alamat = "Jl. Sukajadi No. 131-139, Cipedes, Sukajadi, Bandung",
                TipeCharger = "AC Fast Charging",
                DayaMaxKW = 22,
                JumlahPort = 3,
                Status = "Maintenance",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -6.8894,
                Longitude = 107.5961
            },
            new SpkluLocation
            {
                Id = 8,
                NamaStasiun = "SPKLU PLN UP3 Yogyakarta",
                Kota = "Yogyakarta",
                Alamat = "Jl. Gedongkuning No. 2, Banguntapan, Bantul, D.I. Yogyakarta",
                TipeCharger = "DC Fast Charging",
                DayaMaxKW = 50,
                JumlahPort = 2,
                Status = "Tersedia",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -7.8016,
                Longitude = 110.3986
            },
            new SpkluLocation
            {
                Id = 9,
                NamaStasiun = "SPKLU Rest Area KM 429A Semarang-Solo",
                Kota = "Semarang",
                Alamat = "Rest Area KM 429A, Tol Semarang-Solo, Ungaran, Semarang",
                TipeCharger = "DC Ultra Fast Charging",
                DayaMaxKW = 200,
                JumlahPort = 4,
                Status = "Tersedia",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -7.1268,
                Longitude = 110.4182
            },
            new SpkluLocation
            {
                Id = 10,
                NamaStasiun = "SPKLU PLN UID Bali Renon",
                Kota = "Denpasar",
                Alamat = "Jl. Letda Tantular No. 1, Renon, Denpasar Selatan, Denpasar, Bali",
                TipeCharger = "DC Ultra Fast Charging",
                DayaMaxKW = 150,
                JumlahPort = 4,
                Status = "Tersedia",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -8.6728,
                Longitude = 115.2341
            },
            new SpkluLocation
            {
                Id = 11,
                NamaStasiun = "SPKLU Bandara I Gusti Ngurah Rai",
                Kota = "Badung",
                Alamat = "Area Parkir Domestik Bandara Ngurah Rai, Tuban, Kuta, Badung, Bali",
                TipeCharger = "DC Fast Charging",
                DayaMaxKW = 60,
                JumlahPort = 2,
                Status = "Terpakai",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -8.7482,
                Longitude = 115.1672
            },
            new SpkluLocation
            {
                Id = 12,
                NamaStasiun = "SPKLU Institut Teknologi Sepuluh Nopember (ITS)",
                Kota = "Surabaya",
                Alamat = "Kampus ITS Gedung Rektorat, Sukolilo, Surabaya",
                TipeCharger = "AC Fast Charging",
                DayaMaxKW = 22,
                JumlahPort = 2,
                Status = "Tersedia",
                TarifPerKwh = "Rp 2.466 / kWh",
                Latitude = -7.2824,
                Longitude = 112.7949
            }
        };
    }
}
