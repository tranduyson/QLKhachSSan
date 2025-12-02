using System;
using System.Data;
using HotelManagement.Module;
using DAL;

namespace BLL
{
    public class DichVuBLL
    {
        private DichVuDAL _dal;

        public DichVuBLL()
        {
            _dal = new DichVuDAL();
        }

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        public DataTable GetById(int maDV)
        {
            return _dal.GetById(maDV);
        }

        public string Them(DichVu obj)
        {
            if (obj == null)
                return "Thông tin DichVu không hợp lệ";
            return _dal.Them(obj);
        }

        public string Sua(DichVu obj)
        {
            if (obj == null)
                return "Thông tin DichVu không hợp lệ";
            return _dal.Sua(obj);
        }

        public string Xoa(int maDV)
        {
            return _dal.Xoa(maDV);
        }
    }
}
