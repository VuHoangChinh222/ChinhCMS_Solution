/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using CMS.Backend.Helpers; // Kết nối tới lớp Helper để băm mật khẩu
using CMS.Data;
using CMS.Data.Entities; // Kết nối tới lớp dữ liệu
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Hỗ trợ AsNoTracking
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Authorize(Roles = "Admin")]
    // Controller UserController quản lý các hành động liên quan đến thành viên, ví dụ: hiển thị danh sách thành viên.
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hàm Index trả về danh sách các thành viên từ Database để hiển thị trên giao diện và phân trang
        public IActionResult Index(int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            var query = _context.Users;
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (page > totalPages && totalPages > 0) page = totalPages;

            var users = query.OrderByDescending(u => u.Id)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList(); // Lấy dữ liệu THẬT từ bảng Users trong SQL

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(users);
        }

        // GET: Hiển thị form tạo mới thành viên
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lưu thông tin thành viên mới vào Database
        [HttpPost]
        public IActionResult Create(User model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                var checkExist = _context.Users.Any(u => u.Username == model.Username);
                if (checkExist)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã có người dùng!");
                    return View(model);
                }

                // Thực hiện băm bảo mật mật khẩu trước khi ghi xuống CSDL
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    model.PasswordHash = PasswordHelper.HashPassword(model.PasswordHash);
                }

                // Lưu User mới vào Database
                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Hiển thị form kèm dữ liệu cũ của User để chỉnh sửa
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Thực hiện lưu thay đổi thông tin thành viên
        [HttpPost]
        public IActionResult Edit(User model, string? NewPassword)
        {
            // BƯỚC 1: Loại bỏ xác thực tự động cho cột mật khẩu cũ PasswordHash 
            // vì trên giao diện Edit ta dùng trường ẩn/nhập mật khẩu mới NewPassword chứ không sửa trực tiếp Hash cũ.
            ModelState.Remove("PasswordHash");
            ModelState.Remove("model.PasswordHash");

            // BƯỚC 2: Kiểm tra tính hợp lệ của dữ liệu (Họ tên, tên đăng nhập không trống...)
            if (ModelState.IsValid)
            {
                // BƯỚC 3: Sử dụng AsNoTracking() để lấy dữ liệu gốc của User từ SQL Server.
                // Điều này giúp EF Core không theo dõi đối tượng cũ, tránh xung đột khóa chính khi gọi SaveChanges
                var existingUser = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == model.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // BƯỚC 4: BẢO VỆ CHỐNG TỰ HẠ QUYỀN TRUY CẬP (SELF-DEMOTION GUARD)
                // Nếu Admin đang sửa đổi chính tài khoản của mình, hệ thống sẽ bỏ qua lựa chọn quyền từ giao diện
                // và cưỡng chế đặt lại đúng vai trò cũ từ Database (ngăn Admin tự đổi quyền từ Admin sang Editor).
                if (existingUser.Username == User.Identity.Name)
                {
                    model.Role = existingUser.Role; // Cưỡng chế phục hồi vai trò gốc
                }

                // BƯỚC 5: XỬ LÝ MẬT KHẨU
                // Nếu người dùng nhập mật khẩu mới vào ô "Mật khẩu mới" -> Tiến hành băm mật khẩu mới bằng BCrypt
                if (!string.IsNullOrEmpty(NewPassword))
                {
                    model.PasswordHash = PasswordHelper.HashPassword(NewPassword);
                }
                else
                {
                    // Nếu để trống ô mật khẩu mới -> Lấy lại chuỗi mã hóa mật khẩu cũ đang lưu trong Database
                    model.PasswordHash = existingUser.PasswordHash;
                }

                // BƯỚC 6: Cập nhật thông tin thành viên mới vào Database
                _context.Users.Update(model);
                _context.SaveChanges();
                
                // Trở về trang danh sách thành viên
                return RedirectToAction("Index");
            }

            // BƯỚC 7: Nếu xảy ra lỗi Model, gom các lỗi lại đưa vào ViewBag để lập trình viên chẩn đoán trên giao diện
            ViewBag.ValidationErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return View(model);
        }

        // ==========================================
        // XÓA THÀNH VIÊN (DELETE)
        // ==========================================
        public IActionResult Delete(int id)
        {
            // Tìm thông tin thành viên cần xóa theo ID
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // BẢO VỆ CHỐNG TỰ XÓA TÀI KHOẢN (SELF-DELETION GUARD)
            // So sánh tên tài khoản cần xóa với User.Identity.Name (tên của người dùng đang đăng nhập hệ thống).
            // Nếu trùng khớp, lập tức báo lỗi và ngăn cản hành vi xóa để tránh tình trạng hệ thống bị khóa ngoài (lockout).
            if (user.Username == User.Identity.Name)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự xóa tài khoản quản trị của mình khi đang đăng nhập!";
                return RedirectToAction("Index");
            }

            // Tiến hành xóa thành viên khỏi bảng Users
            _context.Users.Remove(user);
            
            // Đồng bộ dữ liệu xuống SQL Server
            _context.SaveChanges();
            
            // Quay lại trang danh sách thành viên
            return RedirectToAction("Index");
        }
    }
}
