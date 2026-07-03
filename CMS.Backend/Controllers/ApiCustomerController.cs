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
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CMS.Backend.Controllers
{
    // Định nghĩa đường dẫn gọi API cho Khách hàng. Đã cấu hình cứng thành "api/customer"
    // Địa chỉ gọi API thực tế: https://localhost:7291/api/customer
    [Route("api/customer")]
    [ApiController]
    public class ApiCustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Hàm khởi tạo (Constructor): Tiêm ApplicationDbContext và IConfiguration vào
        public ApiCustomerController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        // DTO đại diện cho dữ liệu gửi lên khi cập nhật thông tin
        public class UpdateRequest
        {
            public string FullName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? Password { get; set; }
        }

        // ==========================================
        // 3. API CẬP NHẬT THÔNG TIN KHÁCH HÀNG
        // ==========================================
        // PUT: api/customer/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] UpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Dữ liệu yêu cầu không hợp lệ" });
            }

            try
            {
                // Tìm kiếm khách hàng theo ID
                var customer = _context.Customers.Find(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });
                }

                // Kiểm tra họ tên không để trống
                if (string.IsNullOrWhiteSpace(request.FullName))
                {
                    return BadRequest(new { message = "Họ và tên không được để trống" });
                }

                // Kiểm tra và cập nhật email nếu có thay đổi
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    var emailTrim = request.Email.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(emailTrim, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        return BadRequest(new { message = "Định dạng email không hợp lệ" });
                    }

                    var isDuplicate = _context.Customers.Any(c => c.Email == emailTrim && c.Id != id);
                    if (isDuplicate)
                    {
                        return BadRequest(new { message = "Địa chỉ email này đã được sử dụng bởi một tài khoản khác" });
                    }
                    customer.Email = emailTrim;
                }

                // Kiểm tra định dạng số điện thoại nếu được cung cấp
                if (!string.IsNullOrWhiteSpace(request.Phone) && 
                    !System.Text.RegularExpressions.Regex.IsMatch(request.Phone, @"^(0[3|5|7|8|9])+([0-9]{8})$"))
                {
                    return BadRequest(new { message = "Số điện thoại Việt Nam không hợp lệ" });
                }

                // Cập nhật thông tin cơ bản
                customer.FullName = request.FullName.Trim();
                
                // Chỉ cập nhật Số điện thoại nếu có dữ liệu truyền vào (không bị rỗng)
                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    customer.Phone = request.Phone.Trim();
                }

                // Chỉ cập nhật Địa chỉ nếu có dữ liệu truyền vào (không bị rỗng)
                if (!string.IsNullOrWhiteSpace(request.Address))
                {
                    customer.Address = request.Address.Trim();
                }

                // Cập nhật mật khẩu mới nếu được cung cấp
                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    if (request.Password.Length < 6)
                    {
                        return BadRequest(new { message = "Mật khẩu mới phải chứa ít nhất 6 ký tự" });
                    }
                    customer.Password = PasswordHelper.HashPassword(request.Password);
                }

                _context.Entry(customer).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(new {
                    message = "Cập nhật thông tin tài khoản thành công",
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
                return StatusCode(500, new { message = "Lỗi hệ thống khi cập nhật thông tin khách hàng", error = ex.Message });
            }
        }

        // DTO cho yêu cầu Forgot Password
        public class ForgotPasswordRequest
        {
            public string Email { get; set; }
        }

        // ==========================================
        // 4. API QUÊN MẬT KHẨU KHÁCH HÀNG (Đáp ứng tiêu chí 46)
        // ==========================================
        // POST: api/customer/forgot-password
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Email không được để trống" });
            }

            try
            {
                // Tìm kiếm khách hàng theo email
                var customer = _context.Customers.FirstOrDefault(c => c.Email == request.Email.Trim());
                if (customer == null)
                {
                    return BadRequest(new { message = "Email không tồn tại trong hệ thống!" });
                }

                // Tạo mật khẩu tạm ngẫu nhiên dài 16 ký tự bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt
                var random = new Random();
                const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%&*";
                char[] passwordChars = new char[16];
                for (int i = 0; i < 16; i++)
                {
                    passwordChars[i] = validChars[random.Next(validChars.Length)];
                }
                string tempPassword = new string(passwordChars);

                // Băm mật khẩu tạm bằng BCrypt và lưu vào CSDL
                customer.Password = PasswordHelper.HashPassword(tempPassword);
                _context.Entry(customer).State = EntityState.Modified;
                _context.SaveChanges();

                // Gửi email thật qua SMTP Gmail sử dụng App Password
                try
                {
                    string body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                            <div style='max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 8px; padding: 20px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1);'>
                                <h2 style='color: #4f46e5; border-bottom: 2px solid #4f46e5; padding-bottom: 10px; margin-top: 0;'>KHÔI PHỤC MẬT KHẨU</h2>
                                <p>Xin chào <strong>{customer.FullName}</strong>,</p>
                                <p>Hệ thống nhận được yêu cầu khôi phục mật khẩu cho tài khoản <strong>{customer.Email}</strong> của bạn.</p>
                                
                                <div style='background-color: #f8fafc; border-radius: 6px; padding: 15px; margin: 20px 0; border-left: 4px solid #4f46e5;'>
                                    <h4 style='margin-top: 0; margin-bottom: 10px; color: #1e293b;'>Mật khẩu tạm thời mới của bạn là:</h4>
                                    <p style='font-size: 18px; font-weight: bold; color: #ef4444; margin: 10px 0; letter-spacing: 1px;'>{tempPassword}</p>
                                </div>
                                
                                <p style='font-size: 14px; color: #475569;'>Vui lòng sử dụng mật khẩu tạm này để đăng nhập và thay đổi mật khẩu mới ngay trong mục Quản lý tài khoản của bạn để đảm bảo tính an toàn.</p>
                                <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 20px 0;'/>
                                <p style='font-size: 13px; color: #64748b; margin-bottom: 0;'>Trân trọng,<br/><strong>Đội ngũ Chinh Hoops</strong></p>
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
                        return BadRequest(new { message = "Gửi email thất bại: Vui lòng điền mật khẩu ứng dụng Gmail (SenderPassword) trong file appsettings.json của Backend!" });
                    }

                    using (var mail = new System.Net.Mail.MailMessage())
                    {
                        mail.From = new System.Net.Mail.MailAddress(senderEmail, senderName);
                        mail.To.Add(customer.Email);
                        mail.Subject = "[Chinh Hoops] Yêu cầu khôi phục mật khẩu";
                        mail.Body = body;
                        mail.IsBodyHtml = true;

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
                catch (System.Exception emailEx)
                {
                    System.Console.WriteLine("Lỗi gửi email: " + emailEx.ToString());
                    return StatusCode(500, new { message = "Lỗi khi kết nối gửi email SMTP: " + emailEx.Message + ". Vui lòng chắc chắn bạn đã cấu hình đúng App Password trong appsettings.json." });
                }

                return Ok(new { message = "Mật khẩu tạm thời mới đã được gửi tới email của bạn. Vui lòng kiểm tra hộp thư đến (Inbox hoặc Spam)!" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi khôi phục mật khẩu", error = ex.Message });
            }
        }

        // DTO nhận Token từ Frontend
        public class GoogleLoginRequest
        {
            public string Credential { get; set; }
        }

        // ==========================================
        // 5. API ĐĂNG NHẬP BẰNG GOOGLE
        // ==========================================
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                // 1. Cấu hình xác thực với Client ID
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { "1005144408689-siit18bqj1ub8r6fggtbeh7u207rti7v.apps.googleusercontent.com" }
                };

                // 2. Xác minh token gửi từ ReactJS với máy chủ Google
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Credential, settings);

                if (payload == null)
                    return BadRequest(new { message = "Token Google không hợp lệ hoặc đã hết hạn." });

                // 3. Xử lý Logic Database (Đã có tài khoản hay chưa)
                var customer = _context.Customers.FirstOrDefault(c => c.Email == payload.Email);

                if (customer == null)
                {
                    // Tài khoản chưa tồn tại -> Yêu cầu người dùng cập nhật thông tin
                    return Ok(new
                    {
                        message = "Tài khoản mới, cần cập nhật thông tin",
                        isNewUser = true,
                        draftData = new
                        {
                            email = payload.Email,
                            fullName = payload.Name
                        }
                    });
                }

                // 4. Trả về thông tin đăng nhập thành công
                return Ok(new
                {
                    message = "Đăng nhập Google thành công",
                    customer = new
                    {
                        customer.Id,
                        customer.FullName,
                        customer.Email,
                        customer.Phone,
                        customer.Address
                    }
                });
            }
            catch (InvalidJwtException)
            {
                return BadRequest(new { message = "Xác thực Google thất bại." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi đăng nhập Google", error = ex.Message });
            }
        }

        // DTO cho hoàn tất đăng ký Google
        public class CompleteGoogleRegisterRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Password { get; set; } 
        }

        // ==========================================
        // 6. API HOÀN TẤT ĐĂNG KÝ BẰNG GOOGLE
        // ==========================================
        [HttpPost("register-google")]
        public IActionResult RegisterGoogle([FromBody] CompleteGoogleRegisterRequest request)
        {
            try
            {
                if (_context.Customers.Any(c => c.Email == request.Email))
                {
                    return BadRequest(new { message = "Email này đã được đăng ký trong hệ thống." });
                }

                if (string.IsNullOrWhiteSpace(request.Phone))
                {
                    return BadRequest(new { message = "Vui lòng nhập số điện thoại." });
                }

                if (string.IsNullOrWhiteSpace(request.Address))
                {
                    return BadRequest(new { message = "Vui lòng nhập địa chỉ." });
                }

                string pwd = !string.IsNullOrWhiteSpace(request.Password) ? request.Password : Guid.NewGuid().ToString();
                
                var customer = new Customer
                {
                    FullName = request.FullName?.Trim(),
                    Email = request.Email?.Trim(),
                    Phone = request.Phone?.Trim(),
                    Address = request.Address?.Trim(),
                    Password = PasswordHelper.HashPassword(pwd)
                };
                
                _context.Customers.Add(customer);
                _context.SaveChanges();

                return StatusCode(201, new
                {
                    message = "Hoàn tất tạo tài khoản thành công",
                    customer = new
                    {
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
                return StatusCode(500, new { message = "Lỗi hệ thống khi tạo tài khoản", error = ex.Message });
            }
        }
    }
}
