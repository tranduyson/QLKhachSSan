using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    public static class ApiDataProvider
    {
        private static readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        static ApiDataProvider()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            // Allow overriding the API base URL via environment variable for flexibility when ports change.
            // Usage (PowerShell): $env:HOTEL_API_BASE = 'https://localhost:7160/'; dotnet run
            var envUrl = Environment.GetEnvironmentVariable("HOTEL_API_BASE");
            var baseUrl = string.IsNullOrWhiteSpace(envUrl) ? "https://localhost:7158/" : envUrl;

            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        private static T? GetFromApi<T>(string relativePath)
        {
            try
            {
                var resp = _httpClient.GetAsync(relativePath).GetAwaiter().GetResult();
                if (!resp.IsSuccessStatusCode) return default;
                var json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch(Exception ex)
            {
                // API unreachable or deserialization failed -> return default (null)
                return default;
            }
        }

        private static async Task<T?> PostToApiAsync<T>(string relativePath, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await _httpClient.PostAsync(relativePath, content);
                
                if (!resp.IsSuccessStatusCode) return default;
                
                var responseJson = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        private static async Task<bool> PutToApiAsync(string relativePath, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await _httpClient.PutAsync(relativePath, content);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static async Task<bool> DeleteFromApiAsync(string relativePath)
        {
            try
            {
                var resp = await _httpClient.DeleteAsync(relativePath);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static IReadOnlyList<LoaiPhong> GetLoaiPhongs()
            => GetFromApi<List<LoaiPhong>>("api/LoaiPhong") ?? new List<LoaiPhong>();

        public static LoaiPhong? GetLoaiPhong(int id)
            => GetFromApi<LoaiPhong>($"api/LoaiPhong/{id}");

        public static LoaiPhong? CreateLoaiPhong(LoaiPhong loaiPhong)
            => PostToApiAsync<LoaiPhong>("api/LoaiPhong", loaiPhong).GetAwaiter().GetResult();

        public static bool UpdateLoaiPhong(LoaiPhong loaiPhong)
            => PutToApiAsync($"api/LoaiPhong/{loaiPhong.MaLoaiPhong}", loaiPhong).GetAwaiter().GetResult();

        public static bool DeleteLoaiPhong(int id)
            => DeleteFromApiAsync($"api/LoaiPhong/{id}").GetAwaiter().GetResult();




        public static IReadOnlyList<Phong> GetPhongs()
            => GetFromApi<List<Phong>>("api/Phong") ?? new List<Phong>();

        public static Phong? GetPhong(int id)
            => GetFromApi<Phong>($"api/Phong/{id}");
        public static Phong? CreatePhong(Phong Phong)
           => PostToApiAsync<Phong>("api/Phong", Phong).GetAwaiter().GetResult();

        public static bool UpdatePhong(Phong Phong)
             => PutToApiAsync($"api/Phong/{Phong.MaPhong}", Phong).GetAwaiter().GetResult();

        public static bool DeletePhong(int id)
            => DeleteFromApiAsync($"api/Phong/{id}").GetAwaiter().GetResult();






        public static IReadOnlyList<KhachHang> GetKhachHangs()
            => GetFromApi<List<KhachHang>>("api/KhachHang") ?? new List<KhachHang>();

        public static KhachHang? GetKhachHang(int id)
            => GetFromApi<KhachHang>($"api/KhachHang/{id}");
        public static KhachHang? CreateKhachHang(KhachHang KhachHang)
           => PostToApiAsync<KhachHang>("api/KhachHang", KhachHang).GetAwaiter().GetResult();

        public static bool UpdateKhachHang(KhachHang KhachHang)
            => PutToApiAsync($"api/KhachHang/{KhachHang.maKhachHang}", KhachHang).GetAwaiter().GetResult();

        public static bool DeleteKhachHang(int id)
            => DeleteFromApiAsync($"api/KhachHang/{id}").GetAwaiter().GetResult();



        public static IReadOnlyList<NhanVien> GetNhanViens()
            => GetFromApi<List<NhanVien>>("api/NhanVien") ?? new List<NhanVien>();

        public static NhanVien? GetNhanVien(int id)
            => GetFromApi<NhanVien>($"api/NhanVien/{id}");
        public static NhanVien? CreateNhanVien(NhanVien nv)
            => PostToApiAsync<NhanVien>("api/NhanVien", nv).GetAwaiter().GetResult();

        public static bool UpdateNhanVien(NhanVien nv)
            => PutToApiAsync($"api/NhanVien/{nv.MaNhanVien}", nv).GetAwaiter().GetResult();

        public static bool DeleteNhanVien(int id)
            => DeleteFromApiAsync($"api/NhanVien/{id}").GetAwaiter().GetResult();



        public static IReadOnlyList<DichVu> GetDichVus()
            => GetFromApi<List<DichVu>>("api/DichVu") ?? new List<DichVu>();

        public static DichVu? GetDichVu(int id)
            => GetFromApi<DichVu>($"api/DichVu/{id}");
        public static DichVu? CreateDichVu(DichVu DichVu)
           => PostToApiAsync<DichVu>("api/DichVu", DichVu).GetAwaiter().GetResult();

        public static bool UpdateDichVu(DichVu DichVu)
            => PutToApiAsync($"api/DichVu/{DichVu.maDichVu}", DichVu).GetAwaiter().GetResult();

        public static bool DeleteDichVu(int id)
            => DeleteFromApiAsync($"api/DichVu/{id}").GetAwaiter().GetResult();



        // Thêm vào class ApiDataProvider
        public static DatPhong? CreateDatPhong(DatPhongRequest request)
            => PostToApiAsync<DatPhong>("api/DatPhong", request).GetAwaiter().GetResult();

        public static bool UpdateDatPhong(DatPhong datPhong)
            => PutToApiAsync($"api/DatPhong/{datPhong.maDatPhong}", datPhong).GetAwaiter().GetResult();

        public static bool DeleteDatPhong(int id)
            => DeleteFromApiAsync($"api/DatPhong/{id}").GetAwaiter().GetResult();

        public static IReadOnlyList<DatPhong> GetDatPhongs()
            => GetFromApi<List<DatPhong>>("api/DatPhong") ?? new List<DatPhong>();

        public static DatPhong? GetDatPhong(int id)
            => GetFromApi<DatPhong>($"api/DatPhong/{id}");

        public static IReadOnlyList<ThanhToan> GetThanhToans()
            => GetFromApi<List<ThanhToan>>("api/ThanhToan") ?? new List<ThanhToan>();

        public static ThanhToan? GetThanhToan(int id)
            => GetFromApi<ThanhToan>($"api/ThanhToan/{id}");

        public static ThanhToan? CreateThanhToan(ThanhToan tt)
            => PostToApiAsync<ThanhToan>("api/ThanhToan", tt).GetAwaiter().GetResult();

        public static bool UpdateThanhToan(ThanhToan tt)
            => PutToApiAsync($"api/ThanhToan/{tt.maThanhToan}", tt).GetAwaiter().GetResult();

        public static bool DeleteThanhToan(int id)
            => DeleteFromApiAsync($"api/ThanhToan/{id}").GetAwaiter().GetResult();
    }
}
