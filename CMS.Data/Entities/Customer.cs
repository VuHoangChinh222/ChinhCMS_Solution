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

        [Required]
        public string FullName { get; set; } // Họ và tên đầy đủ của khách hàng

        [Required]
        [EmailAddress]
        public string Email { get; set; } // Địa chỉ email của khách hàng, có định dạng hợp lệ

        public string? Phone { get; set; } // Số điện thoại của khách hàng, có thể để trống

        public string? Address { get; set; } // Địa chỉ của khách hàng, có thể để trống

        [Required]
        public string Password { get; set; } // Lưu mật khẩu thô theo yêu cầu tối giản

        public virtual ICollection<Order>? Orders { get; set; } // Quan hệ: Một khách hàng có thể có nhiều đơn hàng
    }
}

