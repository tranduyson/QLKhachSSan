using System;
using System.Data;
using HotelManagement.Module;
using DAL;

namespace BLL
{
    public class GiaBLL
    {
        private GiaDAL _dal;

        public GiaBLL()
        {
            _dal = new GiaDAL();
        }

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        public DataTable GetById(int maGia)
        {
            return _dal.GetById(maGia);
        }

        public string Them(Gia obj)
        {
            if (obj == null)
                return "Thông tin Gia không hợp lệ";
            return _dal.Them(obj);
        }

        public string Sua(Gia obj)
        {
            if (obj == null)
                return "Thông tin Gia không hợp lệ";
            return _dal.Sua(obj);
        }

        public string Xoa(int maGia)
        {
            return _dal.Xoa(maGia);
        }
    }
}
