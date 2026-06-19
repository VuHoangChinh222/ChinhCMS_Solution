/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using CMS.Backend.Models;
using CMS.Data; // Kết nối tới lớp quản lý cơ sở dữ liệu
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thư viện hỗ trợ phương thức Include tải kèm bảng liên quan
using System.Diagnostics;
using System.Linq; // Thư viện hỗ trợ các câu lệnh truy vấn LINQ
using Microsoft.AspNetCore.Authorization;

namespace CMS.Backend.Controllers
{
    // Controller HomeController quản lý trang chủ và các trang thông tin chung của hệ thống.
    [Authorize]
    public class HomeController : Controller
    {
        // Khai báo đối tượng kết nối cơ sở dữ liệu
        private readonly ApplicationDbContext _context;

        // Khai báo đối tượng ghi chép nhật ký hệ thống
        private readonly ILogger<HomeController> _logger;

        // Phương thức khởi tạo thực hiện tiêm kết nối cơ sở dữ liệu và trình ghi nhật ký từ hệ thống
        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Hàm Index trả về giao diện trang chủ (Dashboard).
        // Hàm này có nhiệm vụ tổng hợp và phân tích toàn bộ số liệu báo cáo của dự án bao gồm:
        // - Tổng số lượng khách hàng, số đơn hàng, doanh thu thực tế.
        // - Phân tích doanh thu theo 3 mốc thời gian: Hàng ngày (7 ngày qua), Hàng tuần (4 tuần qua), Hàng tháng (6 tháng qua).
        // - Thống kê danh sách 4 sản phẩm bán chạy nhất hệ thống dựa trên số lượng đặt hàng.
        // - Danh sách 3 bài viết tin tức mới nhất đăng trên trang chủ.
        public IActionResult Index()
        {
            // =========================================================================
            // LƯU Ý DÀNH CHO NGƯỜI ĐỌC MÃ NGUỒN:
            // - _context đại diện cho Entity Framework Core DbContext (Cổng kết nối CSDL).
            // - ViewBag là một đối tượng động dùng để truyền dữ liệu tạm thời từ Controller sang Razor View.
            // =========================================================================

            // 1. THỐNG KÊ CƠ BẢN (TỔNG KHÁCH HÀNG, TỔNG ĐƠN HÀNG, TỔNG DOANH THU)
            // Lấy tổng số dòng trong bảng Customers (Khách hàng)
            int totalCustomers = _context.Customers.Count();
            
            // Lấy tổng số dòng trong bảng Orders (Đơn đặt hàng)
            int totalOrders = _context.Orders.Count();
            
            // Tính tổng doanh thu tích lũy bằng cách cộng dồn (Sum) tích của: Số lượng * Đơn giá của từng chi tiết hóa đơn
            decimal totalRevenueDecimal = _context.OrderDetails.Where(od => od.Order.Status == 2).Sum(od => od.Quantity * od.UnitPrice);

            // Đưa các dữ liệu thống kê cơ bản này vào ViewBag để View bên ngoài có thể lấy ra hiển thị
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenueDecimal;

            // 2. THỐNG KÊ DOANH THU HÀNG NGÀY (7 NGÀY QUA)
            // Mốc bắt đầu: Lấy ngày hôm nay lùi lại 6 ngày (tổng cộng 7 ngày bao gồm cả hôm nay)
            var startDateDaily = DateTime.Today.AddDays(-6);
            
            // Truy vấn lấy dữ liệu thô: Tải trước bảng Orders liên kết bằng Include, lọc các đơn hàng có trạng thái đã hoàn thành (2) trong vòng 7 ngày qua
            var dailyRaw = _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Order.Status == 2)
                .Select(od => new { od.Order.OrderDate, od.Quantity, od.UnitPrice })
                .ToList(); // Tải dữ liệu về RAM để tránh các lỗi dịch cú pháp ngày tháng phức tạp của SQL Server

            // Khởi tạo danh sách 7 ngày liên tiếp từ mốc lùi đến hôm nay để tránh bị khuyết ngày (nếu ngày đó có doanh thu = 0)
            var dailyRevenue = Enumerable.Range(0, 7)
                .Select(i => startDateDaily.AddDays(i))
                .Select(date => new
                {
                    // Định dạng nhãn trục hoành đồ thị dạng Ngày/Tháng (Ví dụ: 26/05)
                    Label = date.ToString("dd/MM"),
                    // Lọc các đơn hàng trong ngày đó và tính tổng tiền
                    Value = dailyRaw.Where(r => r.OrderDate.Date == date.Date)
                        .Sum(r => (double)(r.Quantity * r.UnitPrice))
                })
                .ToList();

            // Chuyển đổi danh sách nhãn (Labels) thành chuỗi ngăn cách bởi dấu phẩy bọc trong dấu ngoặc kép để cấp cho Chart.js vẽ đồ thị
            ViewBag.DailyLabels = string.Join(",", dailyRevenue.Select(d => $"\"{d.Label}\""));
            // Chuyển đổi danh sách giá trị doanh thu tương ứng thành chuỗi số phân tách bởi dấu phẩy
            ViewBag.DailyValues = string.Join(",", dailyRevenue.Select(d => d.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)));

            // 3. THỐNG KÊ DOANH THU HÀNG TUẦN (4 TUẦN QUA - TỔNG 28 NGÀY)
            // Mốc bắt đầu: Lấy ngày hôm nay lùi lại 27 ngày
            var startDateWeekly = DateTime.Today.AddDays(-27);
            
            // Lọc dữ liệu thô hóa đơn trong vòng 28 ngày qua có trạng thái đã hoàn thành (2)
            var weeklyRaw = _context.OrderDetails
            .Include(od => od.Order)
            .Where(od => od.Order.OrderDate >= startDateWeekly && od.Order.Status == 2)
            .Select(od => new { od.Order.OrderDate, od.Quantity, od.UnitPrice })
            .ToList();

            // Chia 28 ngày thành 4 nhóm tuần tự (mỗi tuần có 7 ngày)
            var weeklyRevenue = Enumerable.Range(0, 4)
            .Select(i => new
            {
                Start = startDateWeekly.AddDays(i * 7),
                End = startDateWeekly.AddDays(i * 7 + 6)
            })
            .Select(week => new
             {
                // Định dạng nhãn tuần: "Tuần Ngày/Tháng-Ngày/Tháng"
                Label = $"Tuần {week.Start:dd/MM}-{week.End:dd/MM}",
                // Tổng tiền đặt hàng nằm trong khoảng thời gian của tuần đó
                Value = weeklyRaw.Where(r => r.OrderDate.Date >= week.Start.Date && r.OrderDate.Date <= week.End.Date)
                .Sum(r => (double)(r.Quantity * r.UnitPrice))
             })
             .ToList();

            ViewBag.WeeklyLabels = string.Join(",", weeklyRevenue.Select(w => $"\"{w.Label}\""));
            ViewBag.WeeklyValues = string.Join(",", weeklyRevenue.Select(w => w.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)));

            // 4. THỐNG KÊ DOANH THU HÀNG THÁNG (6 THÁNG QUA)
            // Mốc bắt đầu: Ngày đầu tiên của tháng hiện tại lùi lại 5 tháng (đảm bảo đủ 6 tháng gần nhất)
            var startDateMonthly = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);
            
            // Lọc toàn bộ hóa đơn phát sinh trong 6 tháng qua
            var monthlyRaw = _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Order.OrderDate >= startDateMonthly && od.Order.Status == 2)
                .Select(od => new { od.Order.OrderDate, od.Quantity, od.UnitPrice })
                .ToList();

            // Tạo danh sách 6 tháng liên tục
            var monthlyRevenue = Enumerable.Range(0, 6)
            .Select(i => startDateMonthly.AddMonths(i))
            .Select(month => new
            {
                // Định dạng nhãn đồ thị dạng Tháng/Năm (Ví dụ: 05/2026)
                Label = month.ToString("MM/yyyy"),
                // Tính doanh thu của tháng đó
                Value = monthlyRaw.Where(r => r.OrderDate.Month == month.Month && r.OrderDate.Year == month.Year)
                    .Sum(r => (double)(r.Quantity * r.UnitPrice))
             })
             .ToList();

            ViewBag.MonthlyLabels = string.Join(",", monthlyRevenue.Select(m => $"\"{m.Label}\""));
            ViewBag.MonthlyValues = string.Join(",", monthlyRevenue.Select(m => m.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)));

            // 5. TÌM TOP 4 SẢN PHẨM BÁN CHẠY NHẤT HỆ THỐNG (chỉ tính theo đơn hàng đã hoàn thành)
            // Gom nhóm (GroupBy) bảng chi tiết đơn hàng theo các thuộc tính của Sản phẩm (ID, Tên, Giá, Ảnh minh họa)
            var topProducts = _context.OrderDetails
            .Where(od => od.Order.Status == 2)
            .GroupBy(od => new { od.ProductId, od.Product.Name, od.Product.Price, od.Product.ImageUrl })
            .Select(g => new TopProductViewModel
            {
                Id = g.Key.ProductId,
                Name = g.Key.Name,
                Price = g.Key.Price,
                ImageUrl = g.Key.ImageUrl,
                // Tính tổng số lượng đã bán bằng cách cộng dồn cột Quantity trong nhóm sản phẩm đó
                TotalSold = g.Sum(od => od.Quantity)
            })
            // Sắp xếp giảm dần theo tổng lượng bán để đưa sản phẩm bán nhiều nhất lên đầu
            .OrderByDescending(x => x.TotalSold)
            // Chỉ lấy đúng 4 sản phẩm bán chạy nhất
            .Take(4)
            .ToList();

            ViewBag.TopProducts = topProducts;

            // 6. BÀI VIẾT TIN TỨC MỚI NHẤT
            // Lấy 3 bài viết mới nhất đăng trên hệ thống để hiển thị bên dưới bảng điều khiển
            var latestPosts = _context.Posts
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedDate)
            .Take(3)
            .ToList();

            // Truyền danh sách 3 bài viết mới nhất sang giao diện trang chủ
            return View(latestPosts);
        }

        // Hàm hiển thị chính sách bảo mật của trang web
        public IActionResult Privacy()
        {
            return View();
        }

        // Hàm hiển thị trang báo lỗi khi hệ thống xảy ra sự cố ngoài ý muốn
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
