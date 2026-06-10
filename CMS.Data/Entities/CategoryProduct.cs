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
    // Lớp CategoryProduct đại diện cho một danh mục sản phẩm, chứa thông tin về tên danh mục,
    // mô tả và quan hệ với các sản phẩm thuộc danh mục đó.
    public class CategoryProduct
    {
        [Key]
        public int Id { get; set; } // Khóa chính và tự động tăng

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)]
        public string Name { get; set; } = null!; // Tên danh mục (vd: Điện tử, Thời trang)

        public string? Description { get; set; } // Mô tả ngắn về danh mục (vd: Các sản phẩm điện tử như điện thoại, laptop)

        // Đường dẫn hình ảnh đại diện cho danh mục sản phẩm (hỗ trợ hiển thị UI đẹp mắt)
        public string? ImageUrl { get; set; }

        // Quan hệ: Một danh mục có nhiều sản phẩm
        public virtual ICollection<Product>? Products { get; set; }
    }
}

