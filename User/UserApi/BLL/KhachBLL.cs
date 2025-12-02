using System;
using System.Data;
using HotelManagement.Module;
using DAL;

namespace BLL
{
    public class KhachBLL
    {
        private KhachDAL _dal;

        public KhachBLL()
        {
            _dal = new KhachDAL();
        }

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        public DataTable GetById(int maKhach)
        {
            return _dal.GetById(maKhach);
        }

        public string Them(Khach obj)
        {
            if (obj == null)
                return "Thông tin Khach không hợp lệ";
            return _dal.Them(obj);
        }

        public string Sua(Khach obj)
        {
            if (obj == null)
                return "Thông tin Khach không hợp lệ";
            return _dal.Sua(obj);
        }

        public string Xoa(int maKhach)
        {
            return _dal.Xoa(maKhach);
        }
    }
}
