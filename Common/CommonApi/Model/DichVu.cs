using System;

namespace HotelManagement.Module
{
    public class DichVu
    {
        public int MaDV { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public decimal DonGia { get; set; }
        public decimal Thue { get; set; }

        // Constructor mặc định
        public DichVu()
        {
            MaDV = 0;
            Ma = string.Empty;
            Ten = string.Empty;
            DonGia = 0;
            Thue = 0;
        }

        // Constructor có tham số
        public DichVu(string ten, decimal donGia, decimal thue)
        {
            Ten = ten;
            DonGia = donGia;
            Thue = thue;
        }
    }
}
