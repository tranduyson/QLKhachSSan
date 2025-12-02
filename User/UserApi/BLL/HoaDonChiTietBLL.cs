using System;
using System.Data;
using HotelManagement.Module;
using DAL;

namespace BLL
{
    public class HoaDonChiTietBLL
    {
        private HoaDonChiTietDAL _dal;

        public HoaDonChiTietBLL()
        {
            _dal = new HoaDonChiTietDAL();
        }

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
        
        public DataTable GetById(int maCTHD)
        {
            return _dal.GetById(maCTHD);
        }

        public string Them(HoaDonChiTiet obj)
        {
            if (obj == null)
                return "Thông tin HoaDonChiTiet không hợp lệ";
            return _dal.Them(obj);
        }

        public string Sua(HoaDonChiTiet obj)
        {
            if (obj == null)
                return "Thông tin HoaDonChiTiet không hợp lệ";
            return _dal.Sua(obj);
        }

        public string Xoa(int maCTHD)
        {
            return _dal.Xoa(maCTHD);
        }
    }
}
