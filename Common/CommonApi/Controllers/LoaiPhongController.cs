using BLL;
using HotelManagement.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;

namespace HotelManagement.API.Common.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoaiPhongController : ControllerBase
    {
        private readonly LoaiPhongBLL _bll;

        public LoaiPhongController()
        {
            _bll = new LoaiPhongBLL();
        }

        // get loai phòng api
        [HttpGet]
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
                        MaLoaiPhong = row["MaLoaiPhong"],
                        Ma = row["Ma"],
                        Ten = row["Ten"],
                        MoTa = row["MoTa"],
                        SoKhachToiDa = row["SoKhachToiDa"]
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách loại phòng thành công",
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

        //get loại phòng api
        [HttpGet("{id}")]
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
                        message = "Không tìm thấy loại phòng"
                    });
                }

                DataRow row = dt.Rows[0];
                var result = new
                {
                    MaLoaiPhong = row["MaLoaiPhong"],
                    Ma = row["Ma"],
                    Ten = row["Ten"],
                    MoTa = row["MoTa"],
                    SoKhachToiDa = row["SoKhachToiDa"]
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy chi tiết loại phòng thành công",
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

        // post loiaj phòng api
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create([FromBody] LoaiPhong loaiPhong)
        {
            try
            {
                if (loaiPhong == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                string result = _bll.Them(loaiPhong);

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

        //put loại phòng api
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Update(int id, [FromBody] LoaiPhong loaiPhong)
        {
            try
            {
                if (loaiPhong == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                loaiPhong.MaLoaiPhong = id;
                string result = _bll.Sua(loaiPhong);

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

        //Xoa loai phòng api
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