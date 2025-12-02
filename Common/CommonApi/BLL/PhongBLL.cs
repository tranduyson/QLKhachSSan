using System;
using System.Data;
using HotelManagement.Module;
using DAL;

namespace BLL
{
    public class PhongBLL
    {
        private PhongDAL _dal;

        public PhongBLL()
        {
            _dal = new PhongDAL();
        }

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        public DataTable GetById(int maPhong)
        {
            return _dal.GetById(maPhong);
        }

        public string Them(Phong obj)
        {
            if (obj == null)
                return "Thông tin Phong không hợp lệ";
            return _dal.Them(obj);
        }

        public string Sua(Phong obj)
        {
            if (obj == null)
                return "Thông tin Phong không hợp lệ";
            return _dal.Sua(obj);
        }

        public string Xoa(int maPhong)
        {
            return _dal.Xoa(maPhong);
        }


        public string DoiTrangThai(int maPhong, string tinhTrang)
        {
            return _dal.DoiTrangThai(maPhong, tinhTrang);
        }

    }
}
