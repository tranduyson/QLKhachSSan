using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagement.Models
{
    [Obsolete("Sample data removed. Use ApiDataProvider instead.")]
    public static class SampleData
    {
        // Sample data has been removed. Calling these methods will throw so remaining usages are easy to find.
        private static InvalidOperationException Removed() => new InvalidOperationException("Sample data removed. Use ApiDataProvider to fetch live data from the API.");

        public static IReadOnlyList<LoaiPhong> GetLoaiPhongs() => throw Removed();
        public static LoaiPhong? GetLoaiPhong(int id) => throw Removed();

        public static IReadOnlyList<Phong> GetPhongs() => throw Removed();
        public static Phong? GetPhong(int id) => throw Removed();

        public static IReadOnlyList<KhachHang> GetKhachHangs() => throw Removed();
        public static KhachHang? GetKhachHang(int id) => throw Removed();

        public static IReadOnlyList<NhanVien> GetNhanViens() => throw Removed();
        public static NhanVien? GetNhanVien(int id) => throw Removed();

        public static IReadOnlyList<DichVu> GetDichVus() => throw Removed();
        public static DichVu? GetDichVu(int id) => throw Removed();

        public static IReadOnlyList<DatPhong> GetDatPhongs() => throw Removed();
        public static DatPhong? GetDatPhong(int id) => throw Removed();

        public static IReadOnlyList<ThanhToan> GetThanhToans() => throw Removed();
        public static ThanhToan? GetThanhToan(int id) => throw Removed();
    }
}

