using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class DichVuController : Controller
    {
        // ============================
        //          INDEX
        // ============================
        public IActionResult Index()
        {
            var dichVus = ApiDataProvider.GetDichVus();
            return View(dichVus);
        }

        // ============================
        //          DETAILS
        // ============================
        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var dichVu = ApiDataProvider.GetDichVu(id);
            if (dichVu is null)
            {
                TempData["Error"] = "Không tìm thấy dịch vụ với ID này.";
                return RedirectToAction(nameof(Index));
            }

            return View(dichVu);
        }

        // ============================
        //          CREATE GET
        // ============================
        public IActionResult Create()
        {
            return View(new DichVu());
        }

        // ============================
        //          CREATE POST
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DichVu model)
        {
            // Set ID = 0 cho bản ghi mới
            model.maDichVu = 0;

            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(model.TenDichVu))
            {
                ModelState.AddModelError("TenDichVu", "Tên dịch vụ không được để trống.");
            }

            if (model.donGia <= 0)
            {
                ModelState.AddModelError("donGia", "Đơn giá phải lớn hơn 0.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gọi API
            var result = ApiDataProvider.CreateDichVu(model);
            if (result is null)
            {
                TempData["Error"] = "Không thể tạo dịch vụ. Vui lòng kiểm tra API hoặc dữ liệu nhập.";
                return View(model);
            }

            TempData["Message"] = "Đã tạo dịch vụ mới thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        //          EDIT GET
        // ============================
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var dichVu = ApiDataProvider.GetDichVu(id);
            if (dichVu is null)
            {
                TempData["Error"] = "Không tìm thấy dịch vụ với ID này.";
                return RedirectToAction(nameof(Index));
            }

            return View(dichVu);
        }

        // ============================
        //          EDIT POST
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DichVu model)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (id != model.maDichVu)
            {
                TempData["Error"] = "ID không khớp.";
                return RedirectToAction(nameof(Index));
            }

            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(model.TenDichVu))
            {
                ModelState.AddModelError("TenDichVu", "Tên dịch vụ không được để trống.");
            }

            if (model.donGia <= 0)
            {
                ModelState.AddModelError("donGia", "Đơn giá phải lớn hơn 0.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Call API update
            var success = ApiDataProvider.UpdateDichVu(model);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật dịch vụ. Vui lòng thử lại.";
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật dịch vụ thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        //         DELETE POST
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var dichVu = ApiDataProvider.GetDichVu(id);
            if (dichVu is null)
            {
                TempData["Error"] = "Không tìm thấy dịch vụ với ID này.";
                return RedirectToAction(nameof(Index));
            }

            var success = ApiDataProvider.DeleteDichVu(id);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa dịch vụ. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Đã xóa dịch vụ thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
