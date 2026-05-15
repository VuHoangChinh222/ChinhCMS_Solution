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

namespace CMS.Data.Entities
{
    // Lớp User đại diện cho người dùng của hệ thống, bao gồm thông tin đăng nhập và vai trò
    // (quản trị viên hoặc biên tập viên).
    public class User
    {
        public int Id { get; set; } // Khóa chính và tự động tăng
        public string Username { get; set; } // Tên đăng nhập của người dùng
        public string PasswordHash { get; set; } // Mật khẩu hiện chưa được sử dụng hash
        public string FullName { get; set; } // Họ và tên đầy đủ của người dùng
        public string Role { get; set; } // Quản trị viên hoặc Biên tập viên
    }
}

