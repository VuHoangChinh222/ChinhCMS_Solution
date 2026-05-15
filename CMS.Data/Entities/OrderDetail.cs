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
    // Lớp OrderDetail đại diện cho chi tiết của một đơn hàng, chứa thông tin về sản phẩm,
    // số lượng, giá tại thời điểm mua và liên kết tới đơn hàng và sản phẩm tương ứng.
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; } // Khóa chính và tự động tăng

        public int OrderId { get; set; } // Khóa ngoại liên kết tới Order

        public int ProductId { get; set; } //  Khóa ngoại liên kết tới Product

        public int Quantity { get; set; } // Số lượng sản phẩm được đặt trong đơn hàng

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Giá tại thời điểm mua

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}

