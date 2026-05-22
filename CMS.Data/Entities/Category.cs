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
    // Lớp Category đại diện cho một danh mục tin tức, ví dụ: Tin Giáo Dục, Tin Thể Thao, v.v.
    public class Category
    {
        public int Id { get; set; } // Khóa chính và tự động tăng

        //Tên danh mục không được để trống
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        public string Name { get; set; } // Tên danh mục (vd: Tin Giáo Dục)

        // Có thể để trống
        public string? Description { get; set; } // Mô tả ngắn về danh mục (vd: Các tin tức liên quan đến giáo dục)

        // Quan hệ: Một danh mục có nhiều bài viết
        public virtual ICollection<Post> Posts { get; set; }
    }
}

