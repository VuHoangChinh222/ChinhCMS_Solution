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
using System;
using System.Linq;

namespace CMS.Backend.Controllers
{
    // Controller OrdersController quản lý các hành động liên quan đến đơn hàng: hiển thị danh sách, cập nhật trạng thái đơn hàng, chi tiết đơn hàng, xóa đơn hàng.
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. DANH SÁCH ĐƠN HÀNG (INDEX)
        // ==========================================
        public IActionResult Index(int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            var query = _context.Orders.Include(o => o.Customer);
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (page > totalPages && totalPages > 0) page = totalPages;

            var data = query.OrderByDescending(o => o.OrderDate)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            // Thống kê số lượng đơn hàng theo từng trạng thái để làm UI cực đẹp
            ViewBag.PendingCount = _context.Orders.Count(o => o.Status == 0);
            ViewBag.ShippingCount = _context.Orders.Count(o => o.Status == 1);
            ViewBag.CompletedCount = _context.Orders.Count(o => o.Status == 2);
            ViewBag.CancelledCount = _context.Orders.Count(o => o.Status == 3);

            // Tính tổng doanh thu từ các đơn hàng Đã hoàn thành (Status == 2)
            decimal totalRevenue = 0;
            var completedOrderIds = _context.Orders.Where(o => o.Status == 2).Select(o => o.Id).ToList();
            if (completedOrderIds.Any())
            {
                totalRevenue = _context.OrderDetails
                                       .Where(d => completedOrderIds.Contains(d.OrderId))
                                       .Sum(d => d.Quantity * d.UnitPrice);
            }
            ViewBag.TotalRevenue = totalRevenue;

            return View(data);
        }

        // ==========================================
        // 2. CHI TIẾT ĐƠN HÀNG (DETAILS)
        // ==========================================
        public IActionResult Details(int id)
        {
            // Lấy thông tin đơn hàng cùng với Khách hàng
            var order = _context.Orders
                                .Include(o => o.Customer)
                                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng này trong hệ thống.");
            }

            // Lấy danh sách chi tiết đơn hàng, bắt buộc .Include(o => o.Product) để lấy ảnh và tên của sản phẩm
            var orderDetails = _context.OrderDetails
                                       .Include(o => o.Product)
                                       .Where(o => o.OrderId == id)
                                       .ToList();

            // Truyền thông tin đơn hàng sang View bằng ViewBag để vẽ hóa đơn đẹp mắt
            ViewBag.Order = order;

            // Bắt buộc trỏ View về đúng file OrderDetails/Index.cshtml
            return View("~/Views/OrderDetails/Index.cshtml", orderDetails);
        }

        // ==========================================
        // 3. CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG (EDIT)
        // ==========================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _context.Orders
                                .Include(o => o.Customer)
                                .FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng này.");
            }
            return View(order);
        }

        [HttpPost]
        public IActionResult Edit(Order model)
        {
            // Loại bỏ các xác thực không cần thiết đối với liên kết ảo
            ModelState.Remove("Customer");
            ModelState.Remove("OrderDetails");

            if (ModelState.IsValid)
            {
                var existingOrder = _context.Orders.Find(model.Id);
                if (existingOrder != null)
                {
                    int oldStatus = existingOrder.Status;
                    int newStatus = model.Status;

                    // 1. Chuyển từ Chờ duyệt (0) hoặc Hủy (3) sang Đang giao (1) hoặc Đã xong (2) -> Tiến hành trừ kho
                    if ((oldStatus == 0 || oldStatus == 3) && (newStatus == 1 || newStatus == 2))
                    {
                        var details = _context.OrderDetails.Include(d => d.Product).Where(d => d.OrderId == model.Id).ToList();
                        foreach (var detail in details)
                        {
                            if (detail.Product != null && detail.Product.StockQuantity < detail.Quantity)
                            {
                                ModelState.AddModelError("", $"Sản phẩm '{detail.Product.Name}' không đủ số lượng tồn kho để duyệt đơn hàng (Tồn kho hiện tại: {detail.Product.StockQuantity}, Cần: {detail.Quantity}).");
                                return View(model);
                            }
                        }

                        // Trừ số lượng tồn kho của các sản phẩm trong đơn hàng
                        foreach (var detail in details)
                        {
                            if (detail.Product != null)
                            {
                                detail.Product.StockQuantity -= detail.Quantity;
                                _context.Entry(detail.Product).State = EntityState.Modified;
                            }
                        }
                    }
                    // 2. Chuyển từ Đang giao (1) hoặc Đã xong (2) sang Chờ duyệt (0) hoặc Hủy (3) -> Hoàn trả lại kho hàng
                    else if ((oldStatus == 1 || oldStatus == 2) && (newStatus == 0 || newStatus == 3))
                    {
                        var details = _context.OrderDetails.Include(d => d.Product).Where(d => d.OrderId == model.Id).ToList();
                        foreach (var detail in details)
                        {
                            if (detail.Product != null)
                            {
                                detail.Product.StockQuantity += detail.Quantity;
                                _context.Entry(detail.Product).State = EntityState.Modified;
                            }
                        }
                    }

                    existingOrder.Status = model.Status;
                    existingOrder.Notes = model.Notes;
                    _context.Orders.Update(existingOrder);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        // ==========================================
        // 4. XÓA ĐƠN HÀNG (DELETE)
        // ==========================================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders
                                .Include(o => o.Customer)
                                .FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng này.");
            }
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                // Nếu đơn hàng đang ở trạng thái đã trừ kho (1 hoặc 2), hoàn trả lại số lượng cho kho hàng trước khi xóa hẳn đơn
                if (order.Status == 1 || order.Status == 2)
                {
                    var detailsList = _context.OrderDetails.Include(d => d.Product).Where(d => d.OrderId == id).ToList();
                    foreach (var detail in detailsList)
                    {
                        if (detail.Product != null)
                        {
                            detail.Product.StockQuantity += detail.Quantity;
                            _context.Entry(detail.Product).State = EntityState.Modified;
                        }
                    }
                }

                // Xóa các chi tiết đơn hàng trước để tránh lỗi ràng buộc khóa ngoại (Foreign Key Constraint)
                var details = _context.OrderDetails.Where(d => d.OrderId == id);
                _context.OrderDetails.RemoveRange(details);

                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
