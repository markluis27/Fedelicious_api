using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("orders")]
    public class orders
    {
        [Key]
        [Column("order_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int order_id { get; set; }

        [Column("customer_id")]
        [Required]
        public int customer_id { get; set; }

        [Column("location_id")]
        public int? location_id { get; set; }

        [Column("confirmed_by_admin_id")]
        public int? confirmed_by_admin_id { get; set; }

        [Column("order_type")]
        [MaxLength(50)]
        public string? order_type { get; set; }

        [Column("order_status")]
        [MaxLength(50)]
        public string order_status { get; set; } = "Pending";

        [Column("subtotal")]
        public decimal subtotal { get; set; }

        [Column("delivery_fee")]
        public decimal? delivery_fee { get; set; }

        [Column("total_amount")]
        public decimal total_amount { get; set; }

        [Column("downpayment_amount")]
        public decimal? downpayment_amount { get; set; }

        [Column("remaining_balance")]
        public decimal? remaining_balance { get; set; }

        [Column("payment_status")]
        [MaxLength(50)]
        public string payment_status { get; set; } = "Pending";

        [Column("payment_method")]
        [MaxLength(50)]
        public string? payment_method { get; set; }

        [Column("order_date")]
        public DateTime order_date { get; set; }

        [Column("preferred_time")]
        public TimeSpan? preferred_time { get; set; }

        [Column("delivery_address")]
        public string? delivery_address { get; set; }

        [Column("notes")]
        public string? notes { get; set; }
    }
}