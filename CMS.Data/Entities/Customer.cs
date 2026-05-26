/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace CMS.Data.Entities
{
    // Lớp Customer đại diện cho một khách hàng, chứa thông tin về họ tên, email, số điện thoại,
    public class Customer
    {
        [Key]
        public int Id { get; set; } // Khóa chính và tự động tăng

        [Required(ErrorMessage = "Vui lòng nhập họ và tên khách hàng.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        public string FullName { get; set; } // Họ và tên đầy đủ của khách hàng

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng. Ví dụ: khachhang@gmail.com")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Định dạng email không hợp lệ. Ví dụ: khachhang@example.com")]
        public string Email { get; set; } // Địa chỉ email của khách hàng, có định dạng hợp lệ

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại Việt Nam không hợp lệ (phải bắt đầu bằng 03, 05, 07, 08, 09 và gồm 10 chữ số).")]
        public string? Phone { get; set; } // Số điện thoại của khách hàng, có thể để trống

        [StringLength(250, ErrorMessage = "Địa chỉ không được vượt quá 250 ký tự.")]
        public string? Address { get; set; } // Địa chỉ của khách hàng, có thể để trống

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải chứa ít nhất 6 ký tự.")]
        public string Password { get; set; } // Mật khẩu 

        public virtual ICollection<Order>? Orders { get; set; } // Quan hệ: Một khách hàng có thể có nhiều đơn hàng
    }
}

