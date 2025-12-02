using System;

namespace HotelManagement.Module

{
    public class DatPhong
    {
        public int MaDatPhong { get; set; }
        public string MaDat { get; set; }
        public int MaKhach { get; set; }
        public int? MaPhong { get; set; }
        public int MaLoaiPhong { get; set; }
        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }
        public int SoKhach { get; set; }
        public string TrangThai { get; set; }
        public int? NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string GhiChu { get; set; }

        // Constructor mặc định
        public DatPhong()
        {
            MaDatPhong = 0;
            MaDat = string.Empty;
            MaKhach = 0;
            MaPhong = 0;
            MaLoaiPhong = 0;
            NgayNhan = DateTime.Now;
            NgayTra = DateTime.Now;
            SoKhach = 0;
            TrangThai = string.Empty;
            NguoiTao = 0;
            NgayTao = DateTime.Now;
            GhiChu = string.Empty;
        }

        // Constructor có tham số
        public DatPhong(DateTime ngayNhan, DateTime ngayTra, int soKhach, string trangThai, int? nguoiTao, DateTime ngayTao, string ghiChu)
        {
            NgayNhan = ngayNhan;
            NgayTra = ngayTra;
            SoKhach = soKhach;
            TrangThai = trangThai;
            NguoiTao = nguoiTao;
            NgayTao = ngayTao;
            GhiChu = ghiChu;
        }
    }
}
