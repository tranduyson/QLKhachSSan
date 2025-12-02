using System;
using System.Data;
using HotelManagement.Module;
using DAL;

namespace BLL
{
    public class LoaiPhongBLL
    {
        private LoaiPhongDAL _dal;

        public LoaiPhongBLL()
        {
            _dal = new LoaiPhongDAL();
        }

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        public DataTable GetById(int maLoaiPhong)
        {
            return _dal.GetById(maLoaiPhong);
        }

        public string Them(LoaiPhong obj)
        {
            if (obj == null)
                return "Thông tin LoaiPhong không hợp lệ";
            return _dal.Them(obj);
        }

        public string Sua(LoaiPhong obj)
        {
            if (obj == null)
                return "Thông tin LoaiPhong không hợp lệ";
            return _dal.Sua(obj);
        }

        public string Xoa(int maLoaiPhong)
        {
            return _dal.Xoa(maLoaiPhong);
        }
    }
}
