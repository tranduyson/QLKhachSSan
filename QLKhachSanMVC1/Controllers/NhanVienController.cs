using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class NhanVienController : Controller
    {
        public IActionResult Index()
        {
            var list = ApiDataProvider.GetNhanViens();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var nv = ApiDataProvider.GetNhanVien(id);
            if (nv is null)
            {
                TempData["Error"] = "Không tìm thấy nhân viên với ID này.";
                return RedirectToAction(nameof(Index));
            }

            return View(nv);
        }

        public IActionResult Create()
        {
            return View(new NhanVien());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NhanVien model)
        {
            model.MaNhanVien = 0;

            if (string.IsNullOrWhiteSpace(model.TenDangNhap))
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập không được để trống.");
            if (string.IsNullOrWhiteSpace(model.MatKhau))
                ModelState.AddModelError("MatKhau", "Mật khẩu không được để trống.");
            if (string.IsNullOrWhiteSpace(model.HoTen))
                ModelState.AddModelError("HoTen", "Họ tên không được để trống.");

            if (!ModelState.IsValid)
                return View(model);

            var result = ApiDataProvider.CreateNhanVien(model);
            if (result is null)
            {
                TempData["Error"] = "Không thể tạo nhân viên. Vui lòng kiểm tra API hoặc dữ liệu nhập.";
                return View(model);
            }

            TempData["Message"] = "Đã tạo nhân viên thành công.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var nv = ApiDataProvider.GetNhanVien(id);
            if (nv is null)
            {
                TempData["Error"] = "Không tìm thấy nhân viên với ID này.";
                return RedirectToAction(nameof(Index));
            }

            return View(nv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, NhanVien model)
        {
            if (id <= 0 || id != model.MaNhanVien)
            {
                TempData["Error"] = "ID không hợp lệ hoặc không khớp.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(model.TenDangNhap))
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập không được để trống.");
            if (string.IsNullOrWhiteSpace(model.HoTen))
                ModelState.AddModelError("HoTen", "Họ tên không được để trống.");

            if (!ModelState.IsValid)
                return View(model);

            var success = ApiDataProvider.UpdateNhanVien(model);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật nhân viên. Vui lòng thử lại.";
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật nhân viên thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var nv = ApiDataProvider.GetNhanVien(id);
            if (nv is null)
            {
                TempData["Error"] = "Không tìm thấy nhân viên với ID này.";
                return RedirectToAction(nameof(Index));
            }

            var success = ApiDataProvider.DeleteNhanVien(id);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa nhân viên. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Đã xóa nhân viên thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
