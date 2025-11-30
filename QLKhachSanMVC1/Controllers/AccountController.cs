using System.Text.Json;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var vm = new LoginViewModel { ReturnUrl = returnUrl };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Fetch employees from API and validate credentials
            var users = ApiDataProvider.GetNhanViens();
            var user = users.FirstOrDefault(u => string.Equals(u.TenDangNhap, model.Username, StringComparison.OrdinalIgnoreCase)
                                                 && u.MatKhau == model.Password && u.TrangThai == 1);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            // Set a simple auth cookie with user id and name (JSON). For production use proper authentication.
            var payload = JsonSerializer.Serialize(new { Id = user.Id, Name = user.HoTen });
            Response.Cookies.Append("AuthUser", payload, new CookieOptions { HttpOnly = true, Secure = Request.IsHttps });

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("AuthUser"))
            {
                Response.Cookies.Delete("AuthUser");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
