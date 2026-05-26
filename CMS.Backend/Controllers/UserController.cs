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
            // Bỏ qua kiểm tra PasswordHash vì trường này không nhập trên form Sửa (sử dụng NewPassword để thay thế)
            ModelState.Remove("PasswordHash");
            ModelState.Remove("model.PasswordHash");

            if (ModelState.IsValid)
            {
                // 1. Tìm User gốc trong Database để lấy lại mật khẩu cũ nếu cần (sử dụng AsNoTracking)
                var existingUser = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == model.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // 2. Xử lý mật khẩu: Nếu nhập mới thì băm và lưu cái mới, nếu trống thì lấy lại mật khẩu cũ
                if (!string.IsNullOrEmpty(NewPassword))
                {
                    model.PasswordHash = PasswordHelper.HashPassword(NewPassword);
                }
                else
                {
                    model.PasswordHash = existingUser.PasswordHash;
                }

                // 3. Cập nhật vào Database
                _context.Users.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Ghi nhận tất cả các lỗi ModelState (nếu có) để hiển thị chẩn đoán lỗi
            ViewBag.ValidationErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return View(model);
        }

        // POST/GET: Xử lý xóa thành viên dựa trên ID nhận được
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
