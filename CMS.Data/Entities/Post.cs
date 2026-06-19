/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Data.Entities
{
    // Lớp Post đại diện cho một bài viết tin tức, chứa thông tin chi tiết về bài viết như tiêu đề,
    // nội dung, hình ảnh, ngày tạo, và liên kết tới danh mục.
    public class Post
    {
        public int Id { get; set; } // Khóa chính và tự động tăng
        public string Title { get; set; } // Tiêu đề bài viết
        public string Content { get; set; } // Nội dung chi tiết
        public string ImageUrl { get; set; } // Hình ảnh đại diện
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo bài viết, mặc định là ngày hiện tại

        [StringLength(200, ErrorMessage = "Slug không được vượt quá 200 ký tự")]
        public string? Slug { get; set; } = string.Empty; // Đường dẫn thân thiện phục vụ cho SEO URL.

        // Khóa ngoại liên kết tới Category
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}

