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
    // Controller CustomersController quản lý các hành động liên quan đến khách hàng, ví dụ: hiển thị danh sách khách hàng.
    public class CustomersController : Controller
    {
        // Khai báo biến _context để truy cập vào cơ sở dữ liệu thông qua ApplicationDbContext.
        private readonly ApplicationDbContext _context;
        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Hàm Index trả về danh sách các khách hàng mẫu để hiển thị trên giao diện
        public IActionResult Index()
        {
             var data = _context.Customers.ToList(); // Lấy dữ liệu THẬT từ bảng Customers trong SQL
             return View(data);
        }
    }
}
