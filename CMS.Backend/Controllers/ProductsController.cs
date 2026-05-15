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
    // Controller ProductsController quản lý các hành động liên quan đến sản phẩm, ví dụ: hiển thị danh sách sản phẩm.
    public class ProductsController : Controller
    {
        // Khai báo biến _context để truy cập vào cơ sở dữ liệu thông qua ApplicationDbContext.
        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Hàm Index trả về danh sách các sản phẩm mẫu để hiển thị trên giao diện
        public IActionResult Index()
        {
            var data = _context.Products.ToList(); // Lấy dữ liệu THẬT từ bảng Products trong SQL
            return View(data);
        }
    }
}
