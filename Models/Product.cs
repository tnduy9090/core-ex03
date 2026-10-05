using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ex03.Models
{
    [Table("Products")]
    public class Product
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        // Khóa ngoại liên kết tới Category
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // Quan hệ 1 - N với OrderDetail
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
