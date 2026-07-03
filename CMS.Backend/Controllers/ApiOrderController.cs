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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace CMS.Backend.Controllers
{
    // Định nghĩa đường dẫn gọi API cho Đơn hàng. Đã cấu hình cứng thành "api/order"
    // Địa chỉ gọi API thực tế: https://localhost:7291/api/order
    [Route("api/order")]
    [ApiController]
    public class ApiOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        // Hàm khởi tạo (Constructor): Tiêm ApplicationDbContext, IConfiguration và IWebHostEnvironment vào
        public ApiOrderController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _env = env;
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

            // Kiểm tra khách hàng có tồn tại không
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

                        // Giảm số lượng hàng tồn kho (Đáp ứng tiêu chí 30)
                        product.StockQuantity -= item.Quantity;

                        totalOrderAmount += (product.Price * item.Quantity);

                        _context.OrderDetails.Add(orderDetail);
                    }

                    _context.SaveChanges();
                    transaction.Commit(); // Hoàn tất giao dịch thành công

                    // Gửi email xác nhận đơn hàng cho khách hàng
                    // TÁCH API: Không gửi email ở đây nữa để tăng tốc độ tạo đơn hàng (trả về trong 0.1s)
                    // Thay vào đó, Frontend sẽ tự động gọi API /send-mail/{orderId} sau khi nhận được orderId.

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

        // ====================================================
        // API GỬI EMAIL CHẠY NGẦM ĐỂ TĂNG UX FRONTEND
        // ====================================================
        // POST: api/order/send-mail/{orderId}
        [HttpPost("send-mail/{orderId}")]
        public IActionResult TriggerOrderEmail(int orderId)
        {
            try
            {
                var order = _context.Orders.Find(orderId);
                if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng" });

                var customer = _context.Customers.Find(order.CustomerId);
                if (customer == null || string.IsNullOrEmpty(customer.Email)) 
                    return Ok(new { message = "Khách hàng không có email, bỏ qua." });

                var detailsList = _context.OrderDetails.Where(od => od.OrderId == order.Id).ToList();
                decimal totalOrderAmount = detailsList.Sum(od => od.UnitPrice * od.Quantity);

                // Thực hiện thao tác gửi email tốn thời gian (3-5 giây)
                SendOrderConfirmationEmail(customer, order, detailsList, totalOrderAmount);

                return Ok(new { message = "Gửi email xác nhận thành công" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi gửi email", error = ex.Message });
            }
        }

        // Phương thức phụ để gửi email xác nhận đơn hàng qua SMTP Gmail thực tế (Đáp ứng tiêu chí 31)
        private void SendOrderConfirmationEmail(Customer customer, Order order, List<OrderDetail> details, decimal totalAmount)
        {
            try
            {
                // Xây dựng nội dung email dạng HTML chuyên nghiệp
                string body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 8px; padding: 20px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1);'>
                            <h2 style='color: #4f46e5; border-bottom: 2px solid #4f46e5; padding-bottom: 10px; margin-top: 0;'>XÁC NHẬN ĐƠN HÀNG # {order.Id}</h2>
                            <p>Xin chào <strong>{customer.FullName}</strong>,</p>
                            <p>Cảm ơn bạn đã mua sắm tại <strong>Chinh Hoops</strong>. Đơn hàng của bạn đã được tiếp nhận thành công!</p>
                            
                            <div style='background-color: #f8fafc; border-radius: 6px; padding: 15px; margin: 20px 0;'>
                                <h4 style='margin-top: 0; margin-bottom: 10px; color: #1e293b;'>Thông tin giao nhận hàng:</h4>
                                <p style='margin: 4px 0;'><strong>Khách hàng:</strong> {customer.FullName}</p>
                                <p style='margin: 4px 0;'><strong>Số điện thoại:</strong> {customer.Phone}</p>
                                <p style='margin: 4px 0;'><strong>Địa chỉ:</strong> {customer.Address}</p>
                                <p style='margin: 4px 0;'><strong>Ngày đặt:</strong> {order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss")}</p>
                            </div>
                            
                            <h4 style='color: #1e293b; margin-bottom: 10px;'>Chi tiết sản phẩm đã mua:</h4>
                            <table style='width: 100%; border-collapse: collapse;'>
                                <thead>
                                    <tr style='background-color: #e2e8f0; text-align: left;'>
                                        <th style='padding: 8px; border: 1px solid #cbd5e1; text-align: center; width: 80px;'>Hình ảnh</th>
                                        <th style='padding: 8px; border: 1px solid #cbd5e1;'>Sản phẩm</th>
                                        <th style='padding: 8px; border: 1px solid #cbd5e1; text-align: center;'>SL</th>
                                        <th style='padding: 8px; border: 1px solid #cbd5e1; text-align: right;'>Đơn giá</th>
                                        <th style='padding: 8px; border: 1px solid #cbd5e1; text-align: right;'>Thành tiền</th>
                                    </tr>
                                </thead>
                                <tbody>";

                var linkedResources = new List<System.Net.Mail.LinkedResource>();

                foreach (var detail in details)
                {
                    var product = _context.Products.Find(detail.ProductId);
                    var productName = product != null ? product.Name : "Sản phẩm";
                    var imgUrl = product?.ImageUrl ?? "";
                    string imgTag = "";

                    if (!string.IsNullOrEmpty(imgUrl))
                    {
                        // Chuẩn hóa đường dẫn tương đối của ảnh để tìm trên ổ đĩa
                        string relativePath = imgUrl.Replace("/", "\\");
                        if (relativePath.StartsWith("\\"))
                        {
                            relativePath = relativePath.Substring(1);
                        }
                        
                        string absolutePath = System.IO.Path.Combine(_env.WebRootPath, relativePath);
                        
                        if (System.IO.File.Exists(absolutePath))
                        {
                            string cidName = $"img_{detail.ProductId}_{System.Guid.NewGuid().ToString("N").Substring(0, 8)}";
                            var res = new System.Net.Mail.LinkedResource(absolutePath);
                            res.ContentId = cidName;
                            
                            string ext = System.IO.Path.GetExtension(absolutePath).ToLower();
                            if (ext == ".png") res.ContentType.MediaType = "image/png";
                            else if (ext == ".gif") res.ContentType.MediaType = "image/gif";
                            else res.ContentType.MediaType = "image/jpeg";
                            
                            linkedResources.Add(res);
                            imgTag = $"<img src='cid:{cidName}' alt='{productName}' style='width: 60px; height: auto; border-radius: 4px;' />";
                        }
                        else
                        {
                            // Nếu không tìm thấy file ảnh trên đĩa, fallback về địa chỉ localhost tuyệt đối
                            var fullUrl = "https://localhost:7291" + (imgUrl.StartsWith("/") ? imgUrl : "/" + imgUrl);
                            imgTag = $"<img src='{fullUrl}' alt='{productName}' style='width: 60px; height: auto; border-radius: 4px;' />";
                        }
                    }

                    body += $@"
                                    <tr>
                                        <td style='padding: 8px; border: 1px solid #cbd5e1; text-align: center; vertical-align: middle;'>
                                            {imgTag}
                                        </td>
                                        <td style='padding: 8px; border: 1px solid #cbd5e1; vertical-align: middle;'>{productName}</td>
                                        <td style='padding: 8px; border: 1px solid #cbd5e1; text-align: center; vertical-align: middle;'>{detail.Quantity}</td>
                                        <td style='padding: 8px; border: 1px solid #cbd5e1; text-align: right; vertical-align: middle;'>{detail.UnitPrice.ToString("N0")} đ</td>
                                        <td style='padding: 8px; border: 1px solid #cbd5e1; text-align: right; vertical-align: middle;'>{(detail.UnitPrice * detail.Quantity).ToString("N0")} đ</td>
                                    </tr>";
                }

                body += $@"
                                </tbody>
                            </table>
                            
                            <div style='text-align: right; margin-top: 15px; font-size: 16px; font-weight: bold; color: #1e293b;'>
                                Tổng cộng: <span style='color: #ef4444;'>{totalAmount.ToString("N0")} đ</span>
                            </div>
                            
                            <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 20px 0;'/>
                            <p style='font-size: 13px; color: #64748b; margin-bottom: 0;'>Chúng tôi sẽ sớm kiểm duyệt đơn hàng và tiến hành vận chuyển. Mọi thắc mắc vui lòng liên hệ hotline Chinh Hoops.</p>
                        </div>
                    </body>
                    </html>";

                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
                var portStr = _configuration["EmailSettings:Port"] ?? "587";
                int port = int.TryParse(portStr, out int p) ? p : 587;
                var senderName = _configuration["EmailSettings:SenderName"] ?? "Chinh Hoops Support";
                var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "vuhoangchinh222@gmail.com";
                var senderPassword = _configuration["EmailSettings:SenderPassword"] ?? "";

                if (string.IsNullOrWhiteSpace(senderPassword))
                {
                    System.Console.WriteLine("Bỏ qua gửi email đặt hàng: Chưa cấu hình SenderPassword trong appsettings.json.");
                    return;
                }

                using (var mail = new System.Net.Mail.MailMessage())
                {
                    mail.From = new System.Net.Mail.MailAddress(senderEmail, senderName);
                    mail.To.Add(customer.Email);
                    mail.Subject = $"[Chinh Hoops] Xác nhận đặt hàng thành công đơn # {order.Id}";
                    
                    var htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    foreach (var lr in linkedResources)
                    {
                        htmlView.LinkedResources.Add(lr);
                    }
                    mail.AlternateViews.Add(htmlView);

                    using (var smtp = new System.Net.Mail.SmtpClient(smtpServer))
                    {
                        smtp.Port = port;
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                        smtp.Send(mail);
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Bỏ qua lỗi gửi email để tránh treo luồng mua hàng của khách nhưng vẫn log ra console để theo dõi
                System.Console.WriteLine("Lỗi gửi email xác nhận đơn hàng: " + ex.ToString());
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
        // 3. API XEM MỘT ĐƠN HÀNG
        // ==========================================
        // GET: api/order/{id}
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
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
                return Ok(new {
                    order.Id,
                    order.OrderDate,
                    order.Status,
                    StatusText = order.Status == 0 ? "Chờ duyệt" : (order.Status == 1 ? "Đang vận chuyển" : (order.Status == 2 ? "Đã hoàn thành" : "Đã hủy")),
                    order.Notes,
                    Customer = new {
                        order.Customer?.Id,
                        order.Customer?.FullName,
                        order.Customer?.Email,
                        order.Customer?.Phone,
                        order.Customer?.Address 
                    }
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải thông tin đơn hàng", error = ex.Message });
            }
        }

        // ==========================================
        // 4. API XEM CHI TIẾT MỘT ĐƠN HÀNG
        // ==========================================
        // GET: api/order/orderDetail/{id}
        [HttpGet("orderDetail/{id}")]
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
                    StatusText = order.Status == 0 ? "Chờ duyệt" : (order.Status == 1 ? "Đang vận chuyển" : (order.Status == 2 ? "Đã hoàn thành" : "Đã hủy")),
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
