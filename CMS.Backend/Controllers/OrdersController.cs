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
    // Controller OrdersController quản lý các hành động liên quan đến đơn hàng, ví dụ: hiển thị danh sách đơn hàng.
    public class OrdersController : Controller
    {
        // Khai báo biến _context để truy cập vào cơ sở dữ liệu thông qua ApplicationDbContext.
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Hàm Index trả về danh sách các đơn hàng mẫu để hiển thị trên giao diện
        public IActionResult Index()
        {
            var data = _context.Orders.ToList(); // Lấy dữ liệu THẬT từ bảng Orders trong SQL
            return View(data);
        }

        // Hàm Details trả về chi tiết của một đơn hàng dựa trên id
        public IActionResult Details(int id)
        {
            // Lấy danh sách chi tiết đơn hàng (kèm thông tin Product nếu có thể cấu hình sau)
            var orderDetails = _context.OrderDetails
                                       .Where(o => o.OrderId == id)
                                       .ToList();
            
            // Bắt buộc trỏ View về đúng file OrderDetails/Index.cshtml
            return View("~/Views/OrderDetails/Index.cshtml", orderDetails);
        }
    }
}
