/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using Microsoft.AspNetCore.Mvc;
using CMS.Data.Entities; // Kết nối tới lớp dữ liệu

namespace CMS.Backend.Controllers
{
    // Controller UserController quản lý các hành động liên quan đến thành viên, ví dụ: hiển thị danh sách thành viên.
    public class UserController : Controller
    {
        // Hàm Index trả về danh sách các thành viên mẫu để hiển thị trên giao diện
        public IActionResult Index()
        {
            var users = new List<User> {
                new User { Id = 1, Username = "admin", PasswordHash = "123456",
                    FullName = "Vũ Hoàng Chính", Role = "Administrator" },
                new User { Id = 2, Username = "user1",PasswordHash = "123456",
                    FullName = "Trần Thị Bưởi", Role = "Editor"},
                new User { Id = 3, Username = "user2", PasswordHash = "123456",
                    FullName = "Hồ Thị Tư", Role = "Editor"},
                new User { Id = 4, Username = "user3", PasswordHash = "123456",
                    FullName = "Lương Chí Hiền", Role = "Editor"} 
            };
            return View(users);
        }
    }
}
