/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 22/05/2026
 * Version: 1.0
 */

using BCrypt.Net;

namespace CMS.Backend.Helpers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Mã hóa mật khẩu sử dụng thuật toán BCrypt
        /// </summary>
        /// <param name="password">Mật khẩu chưa mã hóa</param>
        /// <returns>Chuỗi mật khẩu đã được băm</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Kiểm tra mật khẩu khớp với chuỗi đã băm hay không
        /// </summary>
        /// <param name="password">Mật khẩu chưa mã hóa</param>
        /// <param name="hashedPassword">Mật khẩu đã được băm</param>
        /// <returns>True nếu khớp, ngược lại False</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }
    }
}
