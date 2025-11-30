namespace HotelManagement.Models
{
    public class LoaiPhong
    {
        public int MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public int SoGiuong { get; set; }
        public decimal DienTich { get; set; }
        public decimal GiaMoiDem { get; set; }

        // Property alias for backward compatibility
        public int Id 
        { 
            get => MaLoaiPhong; 
            set => MaLoaiPhong = value; 
        }
    }

    public class Phong
    {
        public int Id { get; set; }
        public int MaPhong { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public int maLoaiPhong { get; set; }
        public LoaiPhong? LoaiPhong { get; set; }
        public string TinhTrang { get; set; } = "Trong";
    }

    public class KhachHang
    {
        public int maKhachHang { get; set; }
        public string hoTen { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? cccd { get; set; }
        public string? ghiChu { get; set; }
        public string? DiaChi { get; set; }
    }

    public class NhanVien
    {
        // API returns 'maNhanVien' and other fields; keep aliases for compatibility
        public int MaNhanVien { get; set; }
        public int Id
        {
            get => MaNhanVien;
            set => MaNhanVien = value;
        }

        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? ChucVu { get; set; }
        public int TrangThai { get; set; } = 1;
    }

    public class DichVu
    {
        public int maDichVu { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public string? ghiChu { get; set; }
        public string? donViTinh { get; set; }
        public decimal donGia { get; set; }
    }

    public class DatPhong
    {
        public int maDatPhong { get; set; }
        public int maKhachHang { get; set; }
        public KhachHang? khachHang { get; set; }
        public int? maNhanVien { get; set; }
        public NhanVien? nhanVien { get; set; }

        public DateTime ngayDat { get; set; }
        public DateTime ngayNhan { get; set; }
        public DateTime ngayTra { get; set; }

        public string trangThai { get; set; }
        public decimal tongTien { get; set; }

        public List<ChiTietDatPhong>? chiTietDatPhongs { get; set; }
        public List<SuDungDichVu>? suDungDichVus { get; set; }
        public List<ThanhToan>? thanhToans { get; set; }
    }


    public class ChiTietDatPhong
    {
        public int Id { get; set; }
        public int DatPhongId { get; set; }
        public DatPhong? DatPhong { get; set; }
        public int PhongId { get; set; }
        public Phong? Phong { get; set; }
        public decimal DonGia { get; set; }
        public int SoDem { get; set; }
        public decimal ThanhTien => DonGia * SoDem;
        public string SoPhong { get; set; } 

        public int MaLoaiPhong { get; set; } 

        public string TenLoaiPhong { get; set; } 
    }

    public class SuDungDichVu
    {
        public int Id { get; set; }
        public int DatPhongId { get; set; }
        public DatPhong? DatPhong { get; set; }
        public int DichVuId { get; set; }
        public DichVu? DichVu { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
        public string TenDichVu { get; set; }
    }

    public class ThanhToan
    {
        public int maThanhToan { get; set; }
        public int maDatPhong { get; set; }
        public DatPhong? DatPhong { get; set; }
        public decimal SoTien { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public string PhuongThuc { get; set; } = "TienMat";
        public bool TrangThai { get; set; } = false;
        public string? ghiChu { get; set; }
    }

}
