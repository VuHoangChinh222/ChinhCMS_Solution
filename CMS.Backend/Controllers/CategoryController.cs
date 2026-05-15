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
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            // Tạo danh sách các dữ liệu mẫu
            var list = new List<Category> { 
                new Category { Id = 1, Name = "Tin Giáo Dục", Description = "Các tin tức liên quan đến giáo dục" },
                new Category { Id = 2, Name = "Tin Thể Thao", Description = "Các tin tức liên quan đến thể thao" },
                new Category { Id = 3, Name = "Tin Công Nghệ", Description = "Các tin tức liên quan đến công nghệ" }
            };
            return View(list); // Truyền danh sách dữ liệu mẫu lên giao diện
        }
    }
}
