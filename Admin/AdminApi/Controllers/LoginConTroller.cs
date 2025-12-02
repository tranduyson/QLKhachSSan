using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using HotelManagement.Module;
using BLL;
using System.Data;
using System.Security.Claims;

namespace HotelManagement.API.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly NguoiDungBLL _bll;

        public LoginController()
        {
            _bll = new NguoiDungBLL();
        }

        [HttpPost]
        public IActionResult Login([FromBody] NguoiDung login)
        {
            try
            {
                if (login == null)
                    return BadRequest(new { success = false, message = "Thiếu dữ liệu!" });

                DataTable dt = _bll.GetAll();
                var user = dt.AsEnumerable()
                    .FirstOrDefault(r =>
                        r["TenDangNhap"].ToString() == login.TenDangNhap &&
                        r["MatKhau"].ToString() == login.MatKhau);

                if (user == null)
                    return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu" });

                var claims = new List<Claim>
                {
                    new Claim("MaND", user["MaND"].ToString()),
                    new Claim(ClaimTypes.Name, user["TenDangNhap"].ToString()),
                    new Claim(ClaimTypes.Role, user["VaiTro"].ToString())
                };

                var key = Encoding.UTF8.GetBytes("ThisIsA256BitSecretKeyForJWT1234567890");

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(3),
                    Issuer = "yourIssuer",
                    Audience = "yourAudience",
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new
                {
                    success = true,
                    message = "Đăng nhập thành công",
                    token = tokenHandler.WriteToken(token),
                    username = user["TenDangNhap"].ToString(),
                    role = user["VaiTro"].ToString()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
