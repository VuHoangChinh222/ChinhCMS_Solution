/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 26/05/2026
 * Version: 1.0
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using CMS.Data.Entities;

namespace CMS.Backend.Controllers
{
    // Định nghĩa đường dẫn gọi API cho Đơn hàng. Đã cấu hình cứng thành "api/order"
    // Địa chỉ gọi API thực tế: https://localhost:7291/api/order
    [Route("api/order")]
    [ApiController]
    public class ApiOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo (Constructor): Tiêm ApplicationDbContext vào
        public ApiOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO nhận dữ liệu chi tiết giỏ hàng khi checkout
        public class CartItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        // DTO nhận yêu cầu Checkout tổng quát
        public class CheckoutRequestDto
        {
            public int CustomerId { get; set; }
            public string? Notes { get; set; }
            public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        }

        // ==========================================
        // 1. API ĐẶT HÀNG (CHECKOUT ĐƠN HÀNG)
        // ==========================================
        // POST: api/order/checkout
        [HttpPost("checkout")]
        public IActionResult Checkout([FromBody] CheckoutRequestDto request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { message = "Giỏ hàng trống hoặc thông tin đặt hàng không hợp lệ" });
            }

            // Kiểm tra khách hàng có tồn tại không!
            var customerExists = _context.Customers.Any(c => c.Id == request.CustomerId);
            if (!customerExists)
            {
                return BadRequest(new { message = "Khách hàng không tồn tại trong hệ thống" });
            }

            // Sử dụng Transaction để bảo toàn tính toàn vẹn của dữ liệu (tránh trường hợp lỗi giữa chừng)
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Tạo thực thể Đơn hàng mới
                    var order = new Order
                    {
                        CustomerId = request.CustomerId,
                        OrderDate = DateTime.Now,
                        Status = 0, // 0: Chờ duyệt (Mặc định khi vừa đặt hàng)
                        Notes = request.Notes?.Trim()
                    };

                    _context.Orders.Add(order);
                    _context.SaveChanges(); // Lưu trước để sinh tự động Order ID cho các chi tiết đơn hàng

                    decimal totalOrderAmount = 0;

                    // Duyệt qua từng mặt hàng trong giỏ hàng gửi lên
                    foreach (var item in request.Items)
                    {
                        if (item.Quantity <= 0)
                        {
                            return BadRequest(new { message = "Số lượng sản phẩm đặt mua phải lớn hơn 0" });
                        }

                        // Tìm sản phẩm trong CSDL
                        var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product == null)
                        {
                            return BadRequest(new { message = $"Sản phẩm có ID {item.ProductId} không tồn tại" });
                        }

                        // Kiểm tra số lượng hàng tồn kho
                        if (product.StockQuantity < item.Quantity)
                        {
                            return BadRequest(new { 
                                message = $"Sản phẩm '{product.Name}' chỉ còn {product.StockQuantity} sản phẩm trong kho. Không đủ đáp ứng số lượng đặt mua ({item.Quantity})." 
                            });
                        }

                        // Tính tiền và tạo chi tiết đơn hàng
                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = product.Price // Lấy giá sản phẩm tại thời điểm mua hàng
                        };

                        totalOrderAmount += (product.Price * item.Quantity);

                        // Trừ trực tiếp số lượng tồn kho của sản phẩm
                        product.StockQuantity -= item.Quantity;

                        _context.OrderDetails.Add(orderDetail);
                    }

                    _context.SaveChanges();
                    transaction.Commit(); // Hoàn tất giao dịch thành công

                    return StatusCode(201, new {
                        message = "Đặt hàng thành công",
                        orderId = order.Id,
                        orderDate = order.OrderDate,
                        status = "Chờ duyệt",
                        totalAmount = totalOrderAmount
                    });
                }
                catch (System.Exception ex)
                {
                    transaction.Rollback(); // Thu hồi lại toàn bộ thay đổi nếu xảy ra lỗi bất kỳ
                    return StatusCode(500, new { message = "Lỗi hệ thống trong quá trình đặt hàng", error = ex.Message });
                }
            }
        }

        // ==========================================
        // 2. API XEM LỊCH SỬ GIAO DỊCH CỦA KHÁCH HÀNG
        // ==========================================
        // GET: api/order/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public IActionResult GetCustomerOrderHistory(int customerId)
        {
            try
            {
                // Kiểm tra sự tồn tại của khách hàng
                var customerExists = _context.Customers.Any(c => c.Id == customerId);
                if (!customerExists)
                {
                    return NotFound(new { message = "Không tìm thấy thông tin khách hàng này" });
                }

                // Lấy danh sách các đơn hàng, sắp xếp từ mới nhất tới cũ nhất
                var orders = _context.Orders
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => new {
                        o.Id,
                        o.OrderDate,
                        o.Status,
                        StatusText = o.Status == 0 ? "Chờ duyệt" : (o.Status == 1 ? "Đang giao" : "Đã xong"),
                        o.Notes,
                        TotalAmount = o.OrderDetails != null ? o.OrderDetails.Sum(od => od.Quantity * od.UnitPrice) : 0,
                        TotalItems = o.OrderDetails != null ? o.OrderDetails.Sum(od => od.Quantity) : 0
                    })
                    .ToList();

                return Ok(orders);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải lịch sử đơn hàng", error = ex.Message });
            }
        }

        // ==========================================
        // 3. API XEM CHI TIẾT MỘT ĐƠN HÀNG
        // ==========================================
        // GET: api/order/{id}
        [HttpGet("{id}")]
        public IActionResult GetOrderDetail(int id)
        {
            try
            {
                var order = _context.Orders
                    .Include(o => o.Customer)
                    .FirstOrDefault(o => o.Id == id);

                if (order == null)
                {
                    return NotFound(new { message = "Không tìm thấy thông tin đơn hàng này trong hệ thống" });
                }

                var details = _context.OrderDetails
                    .Where(od => od.OrderId == id)
                    .Select(od => new {
                        od.Id,
                        od.ProductId,
                        ProductName = od.Product != null ? od.Product.Name : "Sản phẩm đã bị xóa",
                        ProductImageUrl = od.Product != null ? od.Product.ImageUrl : "",
                        od.Quantity,
                        od.UnitPrice,
                        SubTotal = od.Quantity * od.UnitPrice
                    })
                    .ToList();

                return Ok(new {
                    order.Id,
                    order.OrderDate,
                    order.Status,
                    StatusText = order.Status == 0 ? "Chờ duyệt" : (order.Status == 1 ? "Đang giao" : "Đã xong"),
                    order.Notes,
                    Customer = new {
                        order.Customer?.Id,
                        order.Customer?.FullName,
                        order.Customer?.Email,
                        order.Customer?.Phone,
                        order.Customer?.Address
                    },
                    Items = details,
                    TotalAmount = details.Sum(d => d.SubTotal)
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải chi tiết đơn hàng", error = ex.Message });
            }
        }
    }
}
