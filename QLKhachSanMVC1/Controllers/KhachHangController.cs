using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class KhachHangController : Controller
    {
        public IActionResult Index()
        {
            var khachHangs = ApiDataProvider.GetKhachHangs();
            return View(khachHangs);
        }

        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var khachHang = ApiDataProvider.GetKhachHang(id);
            if (khachHang is null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng với ID này.";
                return RedirectToAction(nameof(Index));
            }

            return View(khachHang);
        }

        public IActionResult Create()
        {
            return View(new KhachHang());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KhachHang model)
        {
            // Set Id = 0 khi tạo mới
            model.maKhachHang = 0;

            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(model.hoTen))
            {
                ModelState.AddModelError("hoTen", "Họ tên không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(model.SoDienThoai))
            {
                ModelState.AddModelError("soDienThoai", "Số điện thoại không được để trống.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = ApiDataProvider.CreateKhachHang(model);
            if (result is null)
            {
                TempData["Error"] = "Không thể tạo khách hàng. Vui lòng kiểm tra API hoặc dữ liệu nhập.";
                return View(model);
            }

            TempData["Message"] = "Đã tạo khách hàng mới thành công.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var khachHang = ApiDataProvider.GetKhachHang(id);
            if (khachHang is null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng với ID này.";
                return RedirectToAction(nameof(Index));
            }

            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, KhachHang model)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (id != model.maKhachHang)
            {
                TempData["Error"] = "ID không khớp.";
                return RedirectToAction(nameof(Index));
            }

            // Đảm bảo ID đúng
            model.maKhachHang = id;

            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(model.hoTen))
            {
                ModelState.AddModelError("hoTen", "Họ tên không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(model.SoDienThoai))
            {
                ModelState.AddModelError("soDienThoai", "Số điện thoại không được để trống.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = ApiDataProvider.UpdateKhachHang(model);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật thông tin khách hàng. Vui lòng thử lại.";
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật khách hàng thành công.";
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

            var khachHang = ApiDataProvider.GetKhachHang(id);
            if (khachHang is null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng với ID này.";
                return RedirectToAction(nameof(Index));
            }

            var success = ApiDataProvider.DeleteKhachHang(id);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa khách hàng. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Đã xóa khách hàng thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
