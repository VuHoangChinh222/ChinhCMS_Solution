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
    // Controller CategoryController quản lý các hành động liên quan đến danh mục tin tức
    // , ví dụ: hiển thị danh sách các danh mục.
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        // "Tiêm" kết nối vào Controller
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy dữ liệu THẬT từ bảng Categories trong SQL
            var data = _context.Categories.ToList();
            return View(data);
        }
    }
}
