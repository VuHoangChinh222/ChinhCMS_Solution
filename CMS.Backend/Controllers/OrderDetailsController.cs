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
    // Controller OrderDetailsController quản lý các hành động liên quan đến chi tiết đơn hàng,
    // ví dụ: hiển thị chi tiết của một đơn hàng cụ thể.
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int id)
        {
            // Lấy danh sách chi tiết đơn hàng (kèm thông tin Product) dựa trên OrderId
            var orderDetails = _context.OrderDetails
                                       .Where(o => o.OrderId == id)
                                       .ToList();
            return View(orderDetails);
        }
    }
}
