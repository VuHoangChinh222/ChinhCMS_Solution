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
using CMS.Backend.Helpers;

namespace CMS.Backend.Controllers
{
    // Định nghĩa đường dẫn gọi API cho Khách hàng. Đã cấu hình cứng thành "api/customer"
    // Địa chỉ gọi API thực tế: https://localhost:7291/api/customer
    [Route("api/customer")]
    [ApiController]
    public class ApiCustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo (Constructor): Tiêm ApplicationDbContext vào
        public ApiCustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO đại diện cho dữ liệu gửi lên khi đăng ký
        public class RegisterRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string Password { get; set; }
        }

        // DTO đại diện cho dữ liệu gửi lên khi đăng nhập
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        // ==========================================
        // 1. API ĐĂNG KÝ TÀI KHOẢN KHÁCH HÀNG
        // ==========================================
        // POST: api/customer/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Dữ liệu yêu cầu không hợp lệ" });
            }

            try
            {
                // Kiểm tra định dạng email bằng Regex (đồng bộ với trigger/data annotations)
                if (string.IsNullOrWhiteSpace(request.Email) || 
                    !System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    return BadRequest(new { message = "Địa chỉ email không đúng định dạng. Ví dụ: khachhang@example.com" });
                }

                // Kiểm tra họ tên không để trống
                if (string.IsNullOrWhiteSpace(request.FullName))
                {
                    return BadRequest(new { message = "Họ và tên không được để trống" });
                }

                // Kiểm tra độ dài mật khẩu
                if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                {
                    return BadRequest(new { message = "Mật khẩu phải chứa ít nhất 6 ký tự" });
                }

                // Kiểm tra định dạng số điện thoại nếu được cung cấp
                if (!string.IsNullOrWhiteSpace(request.Phone) && 
                    !System.Text.RegularExpressions.Regex.IsMatch(request.Phone, @"^(0[3|5|7|8|9])+([0-9]{8})$"))
                {
                    return BadRequest(new { message = "Số điện thoại Việt Nam không hợp lệ (phải bắt đầu bằng 03, 05, 07, 08, 09 và gồm 10 chữ số)" });
                }

                // Kiểm tra trùng lặp email khách hàng
                var emailExists = _context.Customers.Any(c => c.Email == request.Email);
                if (emailExists)
                {
                    return BadRequest(new { message = "Địa chỉ Email này đã được sử dụng trong hệ thống" });
                }

                // Băm mật khẩu bằng BCrypt sử dụng Helper có sẵn
                string hashedPassword = PasswordHelper.HashPassword(request.Password);

                // Khởi tạo thực thể khách hàng mới
                var customer = new Customer
                {
                    FullName = request.FullName.Trim(),
                    Email = request.Email.Trim(),
                    Phone = request.Phone?.Trim(),
                    Address = request.Address?.Trim(),
                    Password = hashedPassword
                };

                _context.Customers.Add(customer);
                _context.SaveChanges();

                return StatusCode(201, new {
                    message = "Đăng ký tài khoản khách hàng thành công",
                    customer = new {
                        customer.Id,
                        customer.FullName,
                        customer.Email,
                        customer.Phone,
                        customer.Address
                    }
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi đăng ký tài khoản khách hàng", error = ex.Message });
            }
        }

        // ==========================================
        // 2. API ĐĂNG NHẬP TÀI KHOẢN KHÁCH HÀNG
        // ==========================================
        // POST: api/customer/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email và mật khẩu không được để trống" });
            }

            try
            {
                // Tìm kiếm khách hàng theo email
                var customer = _context.Customers.FirstOrDefault(c => c.Email == request.Email);
                if (customer == null)
                {
                    // Lỗi trả về đồng bộ chung chung để tăng bảo mật thông tin
                    return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác" });
                }

                // Xác thực mật khẩu đã mã hóa qua BCrypt
                bool isPasswordValid = PasswordHelper.VerifyPassword(request.Password, customer.Password);
                if (!isPasswordValid)
                {
                    return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác" });
                }

                // Đăng nhập thành công, trả về thông tin người dùng
                return Ok(new {
                    message = "Đăng nhập thành công",
                    customer = new {
                        customer.Id,
                        customer.FullName,
                        customer.Email,
                        customer.Phone,
                        customer.Address
                    }
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi đăng nhập", error = ex.Message });
            }
        }
    }
}
