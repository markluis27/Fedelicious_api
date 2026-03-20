using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("orders")]
    public class Orders
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

        [Column("order_type")]
        [Required]
        [MaxLength(50)]
        public string order_type { get; set; }

        [Column("order_status")]
        [Required]
        [MaxLength(50)]
        public string order_status { get; set; } = "pending";

        [Column("subtotal")]
        [Required]
        public decimal subtotal { get; set; }

        [Column("delivery_fee")]
        public decimal? delivery_fee { get; set; }

        [Column("total_amount")]
        [Required]
        public decimal total_amount { get; set; }

        [Column("payment_status")]
        [Required]
        [MaxLength(50)]
        public string payment_status { get; set; } = "GCASH";

        [Column("order_date")]
        [Required]
        public DateTime order_date { get; set; }

        [Column("preferred_time")]
        // BURAHIN MO YUNG [MaxLength(20)] DITO
        public TimeSpan? preferred_time { get; set; }

        [Column("delivery_address")]
        public string? delivery_address { get; set; }
        [Column("payment_method")]
        public string? payment_method { get; internal set; }
    }
}