using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CMS.Data;
using CMS.Backend.Helpers;

namespace CMS.Backend.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Nếu người dùng đã đăng nhập từ trước, tự động chuyển về trang chủ quản trị
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 1. Kiểm tra tài khoản tồn tại trong Database
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            // 2. Xác minh mật khẩu bảo mật sử dụng PasswordHelper (BCrypt)
            if (user != null && PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                // 3. Thiết lập danh tính (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role), // Lưu vai trò (Admin hoặc Editor)
                    new Claim("FullName", user.FullName) // Lưu Họ tên đầy đủ
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 4. Thiết lập thuộc tính Cookie duy trì lâu dài
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Giúp cookie được lưu trên ổ đĩa trình duyệt (không bị mất khi đóng trình duyệt hoặc restart app)
                    ExpiresUtc = System.DateTimeOffset.UtcNow.AddDays(7) // Lưu trong vòng 7 ngày
                };

                // 5. Thực thi Đăng nhập và lưu Cookie vào trình duyệt
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity), 
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            // Ghi nhận thông báo lỗi nếu sai thông tin tài khoản
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            return View();
        }

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            // Hủy phiên đăng nhập Cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
