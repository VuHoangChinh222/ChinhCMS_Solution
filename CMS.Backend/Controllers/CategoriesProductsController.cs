/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using CMS.Data;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    // Controller CategoriesProductsController quản lý các hành động liên quan đến danh mục sản phẩm,
    // ví dụ: hiển thị danh sách các danh mục sản phẩm.
    public class CategoriesProductsController : Controller
    {
        // Khai báo biến _context để truy cập vào cơ sở dữ liệu thông qua ApplicationDbContext.
        private readonly ApplicationDbContext _context;
        public CategoriesProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Hàm Index trả về danh sách các danh mục sản phẩm mẫu để hiển thị trên giao diện
        public IActionResult Index()
        {
            var data = _context.CategoriesProducts.ToList(); // Lấy dữ liệu THẬT từ bảng CategoriesProducts trong SQL
            return View(data);
        }
    }
}
