using System;

namespace HotelManagement.Module


{
    public class LoaiPhong
    {
        public int MaLoaiPhong { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public int SoKhachToiDa { get; set; }

        // Constructor mặc định
        public LoaiPhong()
        {
            MaLoaiPhong = 0;
            Ma = string.Empty;
            Ten = string.Empty;
            MoTa = string.Empty;
            SoKhachToiDa = 0;
        }

        // Constructor có tham số
        public LoaiPhong(string ten, string moTa, int soKhachToiDa)
        {
            Ten = ten;
            MoTa = moTa;
            SoKhachToiDa = soKhachToiDa;
        }
    }
}
