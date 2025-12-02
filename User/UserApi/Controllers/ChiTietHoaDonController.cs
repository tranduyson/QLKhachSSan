using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using BLL;
using HotelManagement.Module;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagement.API.Accounting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChiTietHoaDonController : ControllerBase
    {
        private readonly HoaDonChiTietBLL _bll;

        public ChiTietHoaDonController()
        {
            _bll = new HoaDonChiTietBLL();
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
                        MaCTHD = row["MaCTHD"],
                        MaHD = row["MaHD"],
                        MaDatPhong = row["MaDatPhong"] != DBNull.Value ? row["MaDatPhong"] : null,
                        MaDV = row["MaDV"] != DBNull.Value ? row["MaDV"] : null,
                        SoLuong = row["SoLuong"],
                        DonGia = row["DonGia"],
                        ThanhTien = row["ThanhTien"] != DBNull.Value ? row["ThanhTien"] : null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách chi tiết hóa đơn thành công",
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
                        message = "Không tìm thấy chi tiết hóa đơn"
                    });
                }

                DataRow row = dt.Rows[0];
                var result = new
                {
                    MaCTHD = row["MaCTHD"],
                    MaHD = row["MaHD"],
                    MaDatPhong = row["MaDatPhong"] != DBNull.Value ? row["MaDatPhong"] : null,
                    MaDV = row["MaDV"] != DBNull.Value ? row["MaDV"] : null,
                    SoLuong = row["SoLuong"],
                    DonGia = row["DonGia"],
                    ThanhTien = row["ThanhTien"] != DBNull.Value ? row["ThanhTien"] : null
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
        public IActionResult Create([FromBody] HoaDonChiTiet hoaDonChiTiet)
        {
            try
            {
                if (hoaDonChiTiet == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                hoaDonChiTiet.ThanhTien = hoaDonChiTiet.SoLuong * hoaDonChiTiet.DonGia;

                string result = _bll.Them(hoaDonChiTiet);

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
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult Update(int id, [FromBody] HoaDonChiTiet hoaDonChiTiet)
        {
            try
            {
                if (hoaDonChiTiet == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                hoaDonChiTiet.MaCTHD = id;

                hoaDonChiTiet.ThanhTien = hoaDonChiTiet.SoLuong * hoaDonChiTiet.DonGia;

                string result = _bll.Sua(hoaDonChiTiet);

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
    }
}