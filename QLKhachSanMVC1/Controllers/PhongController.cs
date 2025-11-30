using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagement.Controllers
{
    public class PhongController : Controller
    {
        public IActionResult Index()
        {
            var phongs = ApiDataProvider.GetPhongs();
            var loaiPhongs = ApiDataProvider.GetLoaiPhongs();
            ViewBag.LoaiPhongNames = loaiPhongs.ToDictionary(lp => lp.Id, lp => lp.TenLoaiPhong);
            ViewBag.LoaiPhongPrices = loaiPhongs.ToDictionary(lp => lp.Id, lp => lp.GiaMoiDem);
            return View(phongs);
        }

        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var phong = ApiDataProvider.GetPhong(id);
            if (phong is null)
            {
                TempData["Error"] = "Không tìm thấy phòng với ID này.";
                return RedirectToAction(nameof(Index));
            }

            // Provide LoaiPhong lookup in case API doesn't populate nested LoaiPhong
            var loaiPhongs = ApiDataProvider.GetLoaiPhongs();
            ViewBag.LoaiPhongNames = loaiPhongs.ToDictionary(lp => lp.Id, lp => lp.TenLoaiPhong);
            ViewBag.LoaiPhongPrices = loaiPhongs.ToDictionary(lp => lp.Id, lp => lp.GiaMoiDem);

            return View(phong);
        }

        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new Phong());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Phong model)
        {
            // Set ID = 0 cho bản ghi mới
            model.Id = 0;

            PopulateDropdowns(model.maLoaiPhong, model.TinhTrang);

            // Validate SoPhong
            if (string.IsNullOrWhiteSpace(model.SoPhong))
            {
                ModelState.AddModelError("SoPhong", "Số phòng không được để trống.");
            }
            if (model.maLoaiPhong <= 0)
            {
                ModelState.AddModelError("maLoaiPhong", "Vui lòng chọn loại phòng.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = ApiDataProvider.CreatePhong(model);
            if (result is null)
            {
                TempData["Error"] = "Không thể tạo phòng. Vui lòng kiểm tra API hoặc dữ liệu nhập.";
                return View(model);
            }

            TempData["Message"] = "Đã tạo phòng mới thành công.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var phong = ApiDataProvider.GetPhong(id);
            if (phong is null)
            {
                TempData["Error"] = "Không tìm thấy phòng với ID này.";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(phong.maLoaiPhong, phong.TinhTrang);
            return View(phong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Phong model)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (id != model.Id)
            {
                TempData["Error"] = "ID không khớp.";
                return RedirectToAction(nameof(Index));
            }

            // Ensure MaPhong (used by API) is populated from Id
            model.MaPhong = model.Id;

            PopulateDropdowns(model.maLoaiPhong, model.TinhTrang);

            // Validate
            if (string.IsNullOrWhiteSpace(model.SoPhong))
            {
                ModelState.AddModelError("SoPhong", "Số phòng không được để trống.");
            }
            if (model.maLoaiPhong <= 0)
            {
                ModelState.AddModelError("LoaiPhongId", "Vui lòng chọn loại phòng.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = ApiDataProvider.UpdatePhong(model);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật phòng. Vui lòng thử lại.";
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật phòng thành công.";
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

            var phong = ApiDataProvider.GetPhong(id);
            if (phong is null)
            {
                TempData["Error"] = "Không tìm thấy phòng với ID này.";
                return RedirectToAction(nameof(Index));
            }

            var success = ApiDataProvider.DeletePhong(id);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa phòng. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Đã xóa phòng thành công.";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(int? selectedLoaiPhongId = null, string? selectedTinhTrang = null)
        {
            ViewBag.LoaiPhongs = ApiDataProvider.GetLoaiPhongs()
                .Select(lp => new SelectListItem
                {
                    Value = lp.Id.ToString(),
                    Text = $"{lp.TenLoaiPhong} ({lp.GiaMoiDem.ToString("N0")} đ/đêm)",
                    Selected = selectedLoaiPhongId == lp.Id
                })
                .ToList();

            var statuses = new List<SelectListItem>
            {
                new() { Value = "Trong", Text = "Trống" },
                new() { Value = "DaDat", Text = "Đã đặt" },
                new() { Value = "DangSuDung", Text = "Đang sử dụng" },
                new() { Value = "DangSua", Text = "Đang sửa" }
            };

            foreach (var item in statuses)
            {
                item.Selected = selectedTinhTrang == item.Value;
            }

            ViewBag.TinhTrangList = statuses;
        }
    }
}
