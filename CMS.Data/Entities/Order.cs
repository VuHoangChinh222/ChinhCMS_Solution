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
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Data.Entities
{
    // Lớp Order đại diện cho một đơn hàng, chứa thông tin về ngày đặt hàng, khách hàng,
    // trạng thái đơn hàng và các chi tiết liên quan đến đơn hàng đó.
    public class Order
    {
        [Key]
        public int Id { get; set; } // Khóa chính và tự động tăng

        public DateTime OrderDate { get; set; } = DateTime.Now; // Ngày đặt hàng, mặc định là ngày hiện tại

        public int CustomerId { get; set; } // Khóa ngoại liên kết tới Customer

        public int Status { get; set; } // 0: Chờ duyệt, 1: Đang giao, 2: Đã xong

        public string? Notes { get; set; } // Ghi chú thêm về đơn hàng, có thể để trống

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
