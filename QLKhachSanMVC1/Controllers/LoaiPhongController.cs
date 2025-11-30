using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class LoaiPhongController : Controller
    {
        public IActionResult Index()
        {
            var loaiPhongs = ApiDataProvider.GetLoaiPhongs();
            return View(loaiPhongs);
        }

        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var loaiPhong = ApiDataProvider.GetLoaiPhong(id);
            if (loaiPhong is null)
            {
                TempData["Error"] = "Không tìm thấy loại phòng với ID này.";
                return RedirectToAction(nameof(Index));
            }

            return View(loaiPhong);
        }

        public IActionResult Create()
        {
            return View(new LoaiPhong());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LoaiPhong model)
        {
            // Ensure MaLoaiPhong is 0 for new records
            model.MaLoaiPhong = 0;

            // Validate DienTich
            if (model.DienTich <= 0)
            {
                ModelState.AddModelError("DienTich", "Diện tích phải lớn hơn 0.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = ApiDataProvider.CreateLoaiPhong(model);
            if (result is null)
            {
                TempData["Error"] = "Không thể tạo loại phòng. Vui lòng kiểm tra lại thông tin hoặc đảm bảo API đang chạy.";
                return View(model);
            }

            TempData["Message"] = "Đã tạo loại phòng mới thành công.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var loaiPhong = ApiDataProvider.GetLoaiPhong(id);
            if (loaiPhong is null)
            {
                TempData["Error"] = "Không tìm thấy loại phòng với ID này.";
                return RedirectToAction(nameof(Index));
            }

            return View(loaiPhong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LoaiPhong model)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (id != model.MaLoaiPhong && id != model.Id)
            {
                TempData["Error"] = "ID không khớp.";
                return RedirectToAction(nameof(Index));
            }

            // Ensure MaLoaiPhong is set
            model.MaLoaiPhong = id;

            // Validate DienTich
            if (model.DienTich <= 0)
            {
                ModelState.AddModelError("DienTich", "Diện tích phải lớn hơn 0.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = ApiDataProvider.UpdateLoaiPhong(model);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật loại phòng. Vui lòng thử lại.";
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật loại phòng thành công.";
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

            var loaiPhong = ApiDataProvider.GetLoaiPhong(id);
            if (loaiPhong is null)
            {
                TempData["Error"] = "Không tìm thấy loại phòng với ID này.";
                return RedirectToAction(nameof(Index));
            }

            var success = ApiDataProvider.DeleteLoaiPhong(id);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa loại phòng. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Đã xóa loại phòng thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
