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

        // Hàm Index trả về giao diện trang chủ, hiển thị 3 bài viết mới nhất
        public IActionResult Index()
        {
            // Sử dụng các câu lệnh truy vấn LINQ để lọc dữ liệu:
            // 1. Nạp kèm thông tin danh mục tương ứng bằng Include để tránh lỗi trống dữ liệu ngoài giao diện
            // 2. Sắp xếp danh sách bài viết theo ngày đăng mới nhất giảm dần bằng OrderByDescending
            // 3. Chỉ lấy ra đúng 3 dòng bài viết đầu tiên bằng phương thức Take
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
