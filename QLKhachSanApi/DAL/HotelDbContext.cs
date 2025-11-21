namespace QLKhachSanApi.DAL
{
    /// <summary>
    /// ADO.NET configuration class for Hotel database.
    /// Contains table names and column constants for ADO operations.
    /// </summary>
    public class HotelDbContext
    {
        // Table names
        public const string TableLoaiPhong = "LoaiPhong";
        public const string TablePhong = "Phong";
        public const string TableKhachHang = "KhachHang";
        public const string TableNhanVien = "NhanVien";
        public const string TableDichVu = "DichVu";
        public const string TableDatPhong = "DatPhong";
        public const string TableChiTietDatPhong = "ChiTietDatPhong";
        public const string TableSuDungDichVu = "SuDungDichVu";
        public const string TableThanhToan = "ThanhToan";

        // Column constants for LoaiPhong
        public const string ColMaLoaiPhong = "MaLoaiPhong";
        public const string ColTenLoaiPhong = "TenLoaiPhong";
        public const string ColGiaMoiDem = "GiaMoiDem";
        public const string ColMoTa = "MoTa";
        public const string ColSoGiuong = "SoGiuong";
        public const string ColDienTich = "DienTich";

        // Column constants for Phong
        public const string ColMaPhong = "MaPhong";
        public const string ColSoPhong = "SoPhong";
        public const string ColTinhTrang = "TinhTrang";

        // Column constants for KhachHang
        public const string ColMaKhachHang = "MaKhachHang";
        public const string ColHoTen = "HoTen";
        public const string ColSoDienThoai = "SoDienThoai";
        public const string ColCCCD = "CCCD";
        public const string ColDiaChi = "DiaChi";
        public const string ColGhiChu = "GhiChu";

        // Column constants for NhanVien
        public const string ColMaNhanVien = "MaNhanVien";
        public const string ColTenDangNhap = "TenDangNhap";
        public const string ColMatKhau = "MatKhau";
        public const string ColChucVu = "ChucVu";
        public const string ColTrangThai = "TrangThai";

        // Column constants for DichVu
        public const string ColMaDichVu = "MaDichVu";
        public const string ColTenDichVu = "TenDichVu";
        public const string ColDonGia = "DonGia";
        public const string ColDonViTinh = "DonViTinh";

        // Column constants for DatPhong
        public const string ColMaDatPhong = "MaDatPhong";
        public const string ColNgayDat = "NgayDat";
        public const string ColNgayNhan = "NgayNhan";
        public const string ColNgayTra = "NgayTra";
        public const string ColTongTien = "TongTien";

        // Column constants for ChiTietDatPhong
        public const string ColMaCT = "MaCT";
        public const string ColThanhTien = "ThanhTien";
        public const string ColSoDem = "SoDem";

        // Column constants for SuDungDichVu
        public const string ColMaSuDung = "MaSuDung";
        public const string ColSoLuong = "SoLuong";

        // Column constants for ThanhToan
        public const string ColMaThanhToan = "MaThanhToan";
        public const string ColNgayThanhToan = "NgayThanhToan";
        public const string ColSoTien = "SoTien";
        public const string ColPhuongThuc = "PhuongThuc";
    }
}