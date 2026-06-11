/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 11/06/2026
 * Version: 1.1 (Cập nhật tiếng Việt cho thông báo lỗi)
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.Entities
{
    // Lớp Banner đại diện cho banner quảng cáo hiển thị ở trang chủ và trang sản phẩm
    public class Banner
    {
        [Key]
        public int Id { get; set; } // Khóa chính

        [Required(ErrorMessage = "Vui lòng nhập tên banner.")]
        [StringLength(250, ErrorMessage = "Tên banner không được vượt quá 250 ký tự.")]
        public string Name { get; set; } = string.Empty; // Tên banner

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string? Description { get; set; } // Mô tả chi tiết

        public string? ImageUrl { get; set; } // Đường dẫn hình ảnh (cho phép null để tránh lỗi ModelState khi chưa gán URL ảnh)

        [Required(ErrorMessage = "Vui lòng chọn trạng thái hiển thị.")]
        public int Status { get; set; } = 1; // Trạng thái: 0 là ẩn, 1 là hiện

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo
    }
}
