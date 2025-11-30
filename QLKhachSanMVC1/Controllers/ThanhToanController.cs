using System;
using System.Collections.Generic;
using System.Linq;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelManagement.Controllers
{
    public class ThanhToanController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var payments = ApiDataProvider.GetThanhToans();
                return View(payments);
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var payment = ApiDataProvider.GetThanhToan(id);
            if (payment is null)
            {
                TempData["Error"] = "Không tìm thấy thanh toán này.";
                return RedirectToAction(nameof(Index));
            }

            // Load thông tin đầy đủ của DatPhong nếu chưa có
            if (payment.DatPhong == null && payment.maDatPhong > 0)
            {
                payment.DatPhong = ApiDataProvider.GetDatPhong(payment.maDatPhong);
            }

            return View(payment);
        }

        public IActionResult Create()
        {
            var model = new ThanhToan
            {
                NgayThanhToan = DateTime.Today,
                TrangThai = true
            };

            PopulateDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ThanhToan model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            // Ensure new record id = 0
            model.maThanhToan = 0;

            var created = ApiDataProvider.CreateThanhToan(model);
            if (created is null)
            {
                TempData["Error"] = "Không thể tạo thanh toán. Vui lòng kiểm tra API.";
                return View(model);
            }

            TempData["Message"] = "Đã tạo phiếu thanh toán thành công.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var payment = ApiDataProvider.GetThanhToan(id);
            if (payment is null)
            {
                return NotFound();
            }

            PopulateDropdowns();
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ThanhToan model)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (id != model.maThanhToan)
            {
                TempData["Error"] = "ID không khớp.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            var success = ApiDataProvider.UpdateThanhToan(model);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật thanh toán. Vui lòng thử lại.";
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật thanh toán thành công.";
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

            var payment = ApiDataProvider.GetThanhToan(id);
            if (payment is null)
            {
                TempData["Error"] = "Không tìm thấy thanh toán này.";
                return RedirectToAction(nameof(Index));
            }

            var success = ApiDataProvider.DeleteThanhToan(id);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa thanh toán. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Đã xóa thanh toán thành công.";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            var datPhongs = ApiDataProvider.GetDatPhongs() ?? Enumerable.Empty<DatPhong>();
            
            ViewBag.DatPhongList = new SelectList(
                datPhongs.Select(dp => new 
                {
                    Value = dp.maDatPhong,
                    Text = $"Đơn #{dp.maDatPhong} - Khách: {dp.khachHang?.hoTen} - Tổng tiền: {CalculateTotalAmount(dp):N0}đ"
                }),
                "Value",
                "Text"
            );

            ViewBag.TrangThaiList = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "False", Text = "Chưa thanh toán" },
                    new SelectListItem { Value = "True", Text = "Đã thanh toán" }
                },
                "Value",
                "Text"
            );
        }

        private decimal CalculateTotalAmount(DatPhong datPhong)
        {
            decimal roomTotal = datPhong.chiTietDatPhongs?.Sum(x => x.DonGia * x.SoDem) ?? 0;
            decimal serviceTotal = datPhong.suDungDichVus?.Sum(x => x.DonGia * x.SoLuong) ?? 0;
            return roomTotal + serviceTotal;
        }
    }
}
