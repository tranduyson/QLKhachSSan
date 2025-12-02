using BLL;
using HotelManagement.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GiaController: ControllerBase
    {
        private readonly GiaBLL _bll;

        public GiaController()
        {
            _bll = new GiaBLL();
        }
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
                        MaGia = row["MaGia"],
                        MaLoaiPhong = row["MaLoaiPhong"],
                        TuNgay = row["TuNgay"],
                        DenNgay = row["DenNgay"],
                        GiaMoiDem = row["GiaMoiDem"],
                        GiaMoiGio = row["GiaMoiGio"],
                        GhiChu = row["GhiChu"],

                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách giá thành công",
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
                        message = "Không tìm thấy giá"
                    });
                }

                DataRow row = dt.Rows[0];
                var result = new
                {
                    MaGia = row["MaGia"],
                    MaLoaiPhong = row["MaLoaiPhong"],
                    TuNgay = row["TuNgay"],
                    DenNgay = row["DenNgay"],
                    GiaMoiDem = row["GiaMoiDem"],
                    GiaMoiGio = row["GiaMoiGio"],
                    GhiChu = row["GhiChu"],
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy chi tiết giá thành công",
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
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create([FromBody] Gia gia)
        {
            try
            {
                if (gia == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                string result = _bll.Them(gia);

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
        public IActionResult Update(int id, [FromBody] Gia gia)
        {
            try
            {
                if (gia == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                gia.MaGia = id;
                string result = _bll.Sua(gia);

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
