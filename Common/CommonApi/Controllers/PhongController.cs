using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using BLL;
using HotelManagement.Module;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagement.API.Customer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PhongController : ControllerBase
    {
        private readonly PhongBLL _phongBll;

        public PhongController()
        {
            _phongBll = new PhongBLL();
        }

        // GET: api/Phong
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                DataTable dt = _phongBll.GetAll();
                var result = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new
                    {
                        MaPhong = row["MaPhong"],
                        SoPhong = row["SoPhong"],
                        MaLoaiPhong = row["MaLoaiPhong"],
                        TinhTrang = row["TinhTrang"]
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách phòng thành công",
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

        // GET: api/Phong/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                DataTable dt = _phongBll.GetById(id);
                if (dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy phòng"
                    });
                }

                DataRow row = dt.Rows[0];
                var result = new
                {
                    MaPhong = row["MaPhong"],
                    SoPhong = row["SoPhong"],
                    MaLoaiPhong = row["MaLoaiPhong"],
                    TinhTrang = row["TinhTrang"]
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy chi tiết phòng thành công",
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

        // POST: api/Phong
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create([FromBody] Phong phong)
        {
            try
            {
                if (phong == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                string result = _phongBll.Them(phong);

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

        // PUT: api/Phong/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Update(int id, [FromBody] Phong phong)
        {
            try
            {
                if (phong == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                phong.MaPhong = id;
                string result = _phongBll.Sua(phong);

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

        // DELETE: api/Phong/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(int id)
        {
            try
            {
                string result = _phongBll.Xoa(id);

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

        // PUT: api/Phong/5/TrangThai
        [HttpPut("{id}/TrangThai")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DoiTrangThai(int id, [FromBody] string tinhTrang)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tinhTrang))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Trạng thái không hợp lệ"
                    });
                }

                string result = _phongBll.DoiTrangThai(id, tinhTrang);

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
