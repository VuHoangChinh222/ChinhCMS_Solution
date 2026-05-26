/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CMS.Backend.Controllers
{
    // Controller CustomersController quản lý các hành động liên quan đến khách hàng, ví dụ: hiển thị danh sách, thêm, sửa, xóa khách hàng.
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. HIỂN THỊ DANH SÁCH KHÁCH HÀNG (INDEX)
        // ==========================================
        public IActionResult Index(int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            var query = _context.Customers.Include(c => c.Orders);
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (page > totalPages && totalPages > 0) page = totalPages;

            var data = query.OrderByDescending(c => c.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(data);
        }

        // ==========================================
        // 2. THÊM MỚI KHÁCH HÀNG (CREATE)
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer model)
        {
            // Bỏ qua kiểm tra lỗi Orders liên kết ảo của EF Core
            ModelState.Remove("Orders");

            // Kiểm tra thủ công các trường bắt buộc
            if (string.IsNullOrEmpty(model.FullName))
            {
                ModelState.AddModelError("FullName", "Vui lòng nhập họ và tên khách hàng.");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Vui lòng nhập email khách hàng.");
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu tài khoản.");
            }

            // Kiểm tra trùng Email
            if (!string.IsNullOrEmpty(model.Email) && _context.Customers.Any(c => c.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng trong hệ thống.");
            }

            if (ModelState.IsValid)
            {
                _context.Customers.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // ==========================================
        // 3. CHỈNH SỬA KHÁCH HÀNG (EDIT)
        // ==========================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null)
            {
                return NotFound("Không tìm thấy khách hàng này trong hệ thống.");
            }
            return View(customer);
        }

        [HttpPost]
        public IActionResult Edit(Customer model, string? NewPassword)
        {
            ModelState.Remove("Orders");
            ModelState.Remove("Password");
            ModelState.Remove("model.Password");

            if (string.IsNullOrEmpty(model.FullName))
            {
                ModelState.AddModelError("FullName", "Vui lòng nhập họ và tên khách hàng.");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Vui lòng nhập email khách hàng.");
            }

            // Kiểm tra trùng Email với người khác
            if (!string.IsNullOrEmpty(model.Email) && _context.Customers.Any(c => c.Email == model.Email && c.Id != model.Id))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng bởi một khách hàng khác.");
            }

            if (ModelState.IsValid)
            {
                // Truy vấn thực thể gốc trong Database để giữ mật khẩu cũ nếu không nhập mật khẩu mới
                var existingCustomer = _context.Customers.AsNoTracking().FirstOrDefault(c => c.Id == model.Id);
                if (existingCustomer != null)
                {
                    if (!string.IsNullOrEmpty(NewPassword))
                    {
                        model.Password = NewPassword;
                    }
                    else
                    {
                        model.Password = existingCustomer.Password;
                    }
                }

                _context.Customers.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // ==========================================
        // 4. XÓA KHÁCH HÀNG (DELETE)
        // ==========================================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers
                                   .Include(c => c.Orders)
                                   .FirstOrDefault(c => c.Id == id);
            
            if (customer == null)
            {
                return NotFound("Không tìm thấy khách hàng này trong hệ thống.");
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _context.Customers
                                   .Include(c => c.Orders)
                                   .FirstOrDefault(c => c.Id == id);

            if (customer != null)
            {
                if (customer.Orders != null && customer.Orders.Any())
                {
                    // Lọc kiểm tra xem có đơn hàng nào ở trạng thái "Chờ duyệt" (0) hoặc "Đang giao hàng" (1) không
                    var hasActiveOrders = customer.Orders.Any(o => o.Status == 0 || o.Status == 1);
                    if (hasActiveOrders)
                    {
                        TempData["ErrorMessage"] = "Không thể xóa khách hàng này vì đang có đơn hàng ở trạng thái 'Chờ duyệt' hoặc 'Đang giao hàng'!";
                        return RedirectToAction("Index");
                    }

                    // Nếu khách hàng chỉ có đơn hàng ở trạng thái "Đã xong" (2) hoặc "Đã hủy" (3) thì tiến hành xóa cascading an toàn
                    var historicalOrders = customer.Orders.ToList();
                    foreach (var ord in historicalOrders)
                    {
                        var details = _context.OrderDetails.Where(d => d.OrderId == ord.Id);
                        _context.OrderDetails.RemoveRange(details);
                    }
                    _context.Orders.RemoveRange(historicalOrders);
                }

                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
