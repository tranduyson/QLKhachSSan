using BLL;
using HotelManagement.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Text.Json;

namespace HotelManagement.API.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NguoiDungController : ControllerBase
    {
        private readonly NguoiDungBLL _bll;

        public NguoiDungController()
        {
            _bll = new NguoiDungBLL();
        }

        // GET: api/nguoidung
        [HttpGet]
        [Authorize(Policy = "AdminOnly")] // Chỉ Admin được truy cập
        public IActionResult GetAll()
        {
            try
            {
                DataTable dt = _bll.GetAll();
                var result = new System.Collections.Generic.List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new
                    {
                        MaND = row["MaND"],
                        TenDangNhap = row["TenDangNhap"],
                        MatKhau = row["MatKhau"],
                        HoTen = row["HoTen"],
                        VaiTro = row["VaiTro"]
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách người dùng thành công",
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

        // GET: api/nguoidung/{id}
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")] // Chỉ Admin được truy cập
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
                        message = "Không tìm thấy người dùng"
                    });
                }

                DataRow row = dt.Rows[0];
                var result = new
                {
                    MaND = row["MaND"],
                    TenDangNhap = row["TenDangNhap"],
                    MatKhau = row["MatKhau"],
                    HoTen = row["HoTen"],
                    VaiTro = row["VaiTro"]
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy chi tiết người dùng thành công",
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

        // POST: api/nguoidung
        [HttpPost]
        [Authorize] // Yêu cầu token, bất kỳ role nào
        public IActionResult Create([FromBody] NguoiDung nguoiDung)
        {
            try
            {
                if (nguoiDung == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                string result = _bll.Them(nguoiDung);

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

        // PUT: api/nguoidung/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")] // Chỉ Admin được sửa
        public IActionResult Update(int id, [FromBody] NguoiDung nguoiDung)
        {
            try
            {
                if (nguoiDung == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                nguoiDung.MaND = id;
                string result = _bll.Sua(nguoiDung);

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

        // DELETE: api/nguoidung/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")] // Chỉ Admin được xóa
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

        // PUT: api/nguoidung/{id}/role - Cập nhật VaiTro riêng
        [HttpPut("{id}/role")]
        [Authorize(Policy = "AdminOnly")] // Chỉ Admin được gán role
        public IActionResult UpdateRole(int id, [FromBody] string vaiTro)
        {
            try
            {
                if (string.IsNullOrEmpty(vaiTro))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Vai trò không được để trống"
                    });
                }

                DataTable dt = _bll.GetById(id);
                if (dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy người dùng"
                    });
                }

                NguoiDung nguoiDung = new NguoiDung
                {
                    MaND = id,
                    VaiTro = vaiTro
                };
                string result = _bll.Sua(nguoiDung);

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
                    message = "Cập nhật vai trò thành công"
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
        // PUT: api/nguoidung/changepassword
        [HttpPut("changepassword")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] JsonElement data)
        {
            try
            {
                string tenDangNhap = data.GetProperty("TenDangNhap").GetString();
                string matKhauHienTai = data.GetProperty("MatKhauHienTai").GetString();
                string matKhauMoi = data.GetProperty("MatKhauMoi").GetString();

                if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhauHienTai) || string.IsNullOrEmpty(matKhauMoi))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Thiếu dữ liệu"
                    });
                }

                string result = _bll.DoiMatKhau(tenDangNhap, matKhauHienTai, matKhauMoi);

                if (result.Contains("thành công"))
                {
                    return Ok(new
                    {
                        success = true,
                        message = result
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result
                    });
                }
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

        // POST: api/auth/register - Đăng ký tài khoản mới (không cần xác thực)
        [HttpPost("register")]
        [AllowAnonymous] // Cho phép truy cập không cần token
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validate dữ liệu đầu vào
                if (request == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.TenDangNhap))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tên đăng nhập không được để trống"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.MatKhau))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mật khẩu không được để trống"
                    });
                }

                if (request.MatKhau.Length < 6)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mật khẩu phải có ít nhất 6 ký tự"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.HoTen))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Họ tên không được để trống"
                    });
                }

                // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                DataTable existingUsers = _bll.GetAll();
                foreach (DataRow row in existingUsers.Rows)
                {
                    if (row["TenDangNhap"].ToString().ToLower() == request.TenDangNhap.ToLower())
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Tên đăng nhập đã tồn tại"
                        });
                    }
                }

                // Tạo người dùng mới với vai trò mặc định là "KhachHang"
                NguoiDung nguoiDung = new NguoiDung
                {
                    TenDangNhap = request.TenDangNhap,
                    MatKhau = request.MatKhau, 
                    HoTen = request.HoTen,
                    VaiTro = "Khach" 
                };

                string result = _bll.Them(nguoiDung);

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
                    message = "Đăng ký tài khoản thành công",
                    data = new
                    {
                        TenDangNhap = nguoiDung.TenDangNhap,
                        HoTen = nguoiDung.HoTen,
                        VaiTro = nguoiDung.VaiTro
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
    }






}