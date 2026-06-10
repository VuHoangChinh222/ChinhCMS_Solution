/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 10/06/2026
 * Version: 1.0
 */

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CMS.Backend.Helpers
{
    public static class SlugHelper
    {
        // Hàm chuyển đổi chuỗi tiếng Việt có dấu thành chuỗi không dấu phục vụ cho SEO URL
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return string.Empty;

            // Chuyển về chữ thường
            string str = phrase.ToLower();

            // Thay thế các ký tự tiếng Việt có dấu thành không dấu
            str = Regex.Replace(str, @"[áàảãạăắằẳẵặâấầẩẫậ]", "a");
            str = Regex.Replace(str, @"[éèẻẽẹêếềểễệ]", "e");
            str = Regex.Replace(str, @"[íìỉĩị]", "i");
            str = Regex.Replace(str, @"[óòỏõọôốồổỗộơớờởỡợ]", "o");
            str = Regex.Replace(str, @"[úùủũụưứừửữự]", "u");
            str = Regex.Replace(str, @"[ýỳỷỹỵ]", "y");
            str = Regex.Replace(str, @"[đ]", "d");

            // Loại bỏ tất cả các ký tự không phải chữ cái thường, số, dấu khoảng trắng hoặc dấu gạch ngang
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // Thu gọn khoảng trắng thừa
            str = Regex.Replace(str, @"\s+", " ").Trim();

            // Thay khoảng trắng bằng dấu gạch ngang
            str = str.Replace(" ", "-");

            // Thu gọn các dấu gạch ngang liên tiếp
            str = Regex.Replace(str, @"-+", "-");

            return str;
        }
    }
}
