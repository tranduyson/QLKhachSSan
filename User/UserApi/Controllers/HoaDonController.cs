using BLL;
using HotelManagement.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;


namespace HotelManagement.API.Accounting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HoaDonController : ControllerBase
    {
        private readonly HoaDonBLL _bll;

        public HoaDonController()
        {
            _bll = new HoaDonBLL();
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult GetAll()
        {
            try
            {
                DataTable dt = _bll.GetAll();
                var result = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new
                    {
                        MaHD = row["MaHD"],
                        SoHD = row["SoHD"],
                        MaKhach = row["MaKhach"],
                        MaND = row["MaND"],
                        NgayLap = row["NgayLap"],
                        TongTien = row["TongTien"],
                        HinhThucThanhToan = row["HinhThucThanhToan"],
                        SoTienDaTra = row["SoTienDaTra"],
                        SoTienConNo = row["SoTienConNo"]
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách hóa đơn thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult GetById(int id)
        {
            try
            {
                DataTable dt = _bll.GetById(id);

                if (dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy hóa đơn"
                    });
                }

                DataRow row = dt.Rows[0];
                var result = new
                {
                    MaHD = row["MaHD"],
                    SoHD = row["SoHD"],
                    MaKhach = row["MaKhach"],
                    MaND = row["MaND"],
                    NgayLap = row["NgayLap"],
                    TongTien = row["TongTien"],
                    HinhThucThanhToan = row["HinhThucThanhToan"],
                    SoTienDaTra = row["SoTienDaTra"],
                    SoTienConNo = row["SoTienConNo"]
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy chi tiết hóa đơn thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult Create([FromBody] HoaDon hoaDon)
        {
            try
            {
                if (hoaDon == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                string result = _bll.Them(hoaDon);

                if (result.Contains("Lỗi"))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Update(int id, [FromBody] HoaDon hoaDon)
        {
            try
            {
                if (hoaDon == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                hoaDon.MaHD = id;
                string result = _bll.Sua(hoaDon);

                if (result.Contains("Lỗi"))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }



        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(int id)
        {
            try
            {
                string result = _bll.Xoa(id);

                if (result.Contains("Lỗi"))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }



            [HttpGet("Getpayment/{id}")]
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult GetPayment(int id)
            {
                try
                {
                    var hoaDonChiTiet = _bll.GetPayment(id);

                    if (hoaDonChiTiet == null || hoaDonChiTiet.Rows.Count == 0)
                    {
                        return NotFound(new
                        {
                            success = false,
                            message = "Không tìm thấy hóa đơn hoặc hóa đơn không có chi tiết."
                        });
                    }

                    var chiTietList = hoaDonChiTiet.AsEnumerable().Select(row => new
                    {
                        MaCTHD = row["MaCTHD"],
                        MaHD = row["MaHD"],
                        MaDatPhong = row["MaDatPhong"] == DBNull.Value ? null : row["MaDatPhong"],
                        MaDV = row["MaDV"] == DBNull.Value ? null : row["MaDV"],
                        SoLuong = row["SoLuong"],
                        DonGia = row["DonGia"],
                        ThanhTien = row["ThanhTien"],
                        SoHD = row["SoHD"],
                        NgayLap = row["NgayLap"],
                        TongTien = row["TongTien"],
                        HinhThucThanhToan = row["HinhThucThanhToan"],
                        SoTienDaTra = row["SoTienDaTra"],
                        SoTienConNo = row["SoTienConNo"]
                    }).ToList();

                    decimal tongTien = chiTietList.Sum(x => Convert.ToDecimal(x.ThanhTien));

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin hóa đơn thành công.",
                    data = new
                    {
                        MaHD = id,
                        TongTien = tongTien,
                        ChiTiet = chiTietList
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }

        [HttpPut("payment/{maHD}")]
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult Payment(int maHD, [FromBody] JsonElement request)
        {
            try
            {
                decimal soTienTra = request.GetProperty("soTienTra").GetDecimal();
                string hinhThucThanhToan = request.GetProperty("hinhThucThanhToan").GetString();
                string tinhTrang = request.TryGetProperty("tinhTrang", out JsonElement tt) ? tt.GetString() : "SanSang";

                string result = _bll.Payment(
                    maHD,
                    tinhTrang,
                    soTienTra,
                    hinhThucThanhToan
                );

                if (result.Contains("Lỗi"))
                {
                    return BadRequest(new { success = false, message = result });
                }

                return Ok(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [HttpGet("BaoCao")]
        [Authorize(Policy = "AdminOrKeToan")]
        public IActionResult BaoCao(
        [FromQuery] string loai = "thang",
        [FromQuery] int? nam = null,
        [FromQuery] int? thang = null,
        [FromQuery] int? tuan = null,
         [FromQuery] int? ngay = null
)
        {
            try
            {
                DataTable dt = _bll.BaoCao();
                var query = dt.AsEnumerable();

                if (nam.HasValue)
                    query = query.Where(r => ((DateTime)r["NgayLap"]).Year == nam.Value);
                if (thang.HasValue)
                    query = query.Where(r => ((DateTime)r["NgayLap"]).Month == thang.Value);
                if (ngay.HasValue)
                    query = query.Where(r => ((DateTime)r["NgayLap"]).Day == ngay.Value);

                if (loai.ToLower() == "quy" && thang.HasValue)
                {
                    int quy = (thang.Value - 1) / 3 + 1;
                    query = query.Where(r =>
                    {
                        int month = ((DateTime)r["NgayLap"]).Month;
                        int monthQuy = (month - 1) / 3 + 1;
                        return monthQuy == quy;
                    });
                }

                if (loai.ToLower() == "tuan" && tuan.HasValue && nam.HasValue)
                {
                    var cul = System.Globalization.CultureInfo.CurrentCulture;
                    query = query.Where(r =>
                    {
                        DateTime dtNgay = (DateTime)r["NgayLap"];
                        int weekNum = cul.Calendar.GetWeekOfYear(dtNgay, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                        return weekNum == tuan.Value && dtNgay.Year == nam.Value;
                    });
                }

                decimal tongTien = query.Sum(r => Convert.ToDecimal(r["ThanhTien"]));
                int soPhong = query.Count(r => r["MaDatPhong"] != DBNull.Value);
                int soHoaDon = query.Select(r => r["MaHD"]).Distinct().Count();

                return Ok(new
                {
                    success = true,
                    message = "Lấy báo cáo tổng hợp thành công",
                    data = new
                    {
                        tongTien,
                        soPhong,
                        soHoaDon,
                        thoiGian = loai.ToLower()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy báo cáo: " + ex.Message
                });
            }
        }




    }
}