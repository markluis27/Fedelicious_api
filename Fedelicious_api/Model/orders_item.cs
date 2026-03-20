using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("order_items")]
    public class order_items
    {
        [Key]
        [Column("order_item_id")]
        public int order_item_id { get; set; }

        [Column("order_id")]
        [Required]
        public int order_id { get; set; }

        [Column("menu_id")]
        [Required]
        public int menu_id { get; set; }

        [Column("quantity")]
        [Required]
        public int quantity { get; set; }

        [Column("price")]
        [Required]
        public decimal price { get; set; }

        [Column("subtotal")]
        [Required]
        public decimal subtotal { get; set; }

    }
}
