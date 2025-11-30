using System;
using System.Linq;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(int? month, int? year)
        {
            var rooms = ApiDataProvider.GetPhongs();
            var bookings = ApiDataProvider.GetDatPhongs();

            var today = DateTime.Today;
            var currentMonth = month ?? today.Month;
            var currentYear = year ?? today.Year;

            // Normalize room statuses and compute canonical breakdown so dashboard consistently displays counts
            int totalRooms = rooms.Count;
            int availableRooms = 0;
            var breakdown = new System.Collections.Generic.Dictionary<string, int>(System.StringComparer.OrdinalIgnoreCase);

            string Normalize(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return string.Empty;
                var t = s.Trim().ToLowerInvariant();
                var normalized = t.Normalize(System.Text.NormalizationForm.FormD);
                var sb = new System.Text.StringBuilder();
                foreach (var ch in normalized)
                {
                    var cat = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
                    if (cat != System.Globalization.UnicodeCategory.NonSpacingMark)
                    {
                        sb.Append(ch);
                    }
                }
                t = sb.ToString();
                t = System.Text.RegularExpressions.Regex.Replace(t, "[^a-z0-9]", "");
                return t;
            }

            foreach (var r in rooms)
            {
                var n = Normalize(r.TinhTrang);
                string key;
                if (string.IsNullOrEmpty(n) || n.Contains("trong"))
                {
                    key = "Trong";
                    availableRooms++;
                }
                else if (n.Contains("dangsudung") || n.Contains("sudung"))
                {
                    key = "DangSuDung";
                }
                else if (n.Contains("baotri") || n.Contains("sua") || n.Contains("dangsua"))
                {
                    key = "BaoTri";
                }
                else if (n.Contains("dadat") || n.Contains("dat"))
                {
                    key = "DaDat";
                }
                else
                {
                    // fallback to Trong
                    key = "Trong";
                    availableRooms++;
                }

                if (breakdown.ContainsKey(key)) breakdown[key]++; else breakdown[key] = 1;
            }

            var viewModel = new DashboardViewModel
            {
                TotalRooms = totalRooms,
                AvailableRooms = availableRooms,
                TodaysBookings = bookings.Count(dp => dp.ngayDat.Date == today),
                MonthlyRevenue = bookings.Where(dp => dp.ngayDat.Month == currentMonth && dp.ngayDat.Year == currentYear).Sum(dp => dp.tongTien),
                RecentBookings = bookings.OrderByDescending(dp => dp.ngayDat).Take(5).ToList(),
                RoomStatusBreakdown = breakdown
            };

            // Expose selected month/year to view
            ViewBag.SelectedMonth = currentMonth;
            ViewBag.SelectedYear = currentYear;

            return View(viewModel);
        }
    }
}
