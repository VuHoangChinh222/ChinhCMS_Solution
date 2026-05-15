/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using CMS.Data;
using CMS.Data.Entities; // Kết nối tới lớp dữ liệu
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    // Controller UserController quản lý các hành động liên quan đến thành viên, ví dụ: hiển thị danh sách thành viên.
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Hàm Index trả về danh sách các thành viên mẫu để hiển thị trên giao diện
        public IActionResult Index()
        {
            var users = _context.Users.ToList(); // Lấy dữ liệu THẬT từ bảng Users trong SQL
            return View(users);
        }
    }
}
