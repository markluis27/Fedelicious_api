using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("payments")]
    public class payments
    {
        [Key]
        [Column("payment_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int payment_id { get; set; }

        [Column("order_id")]
        public int? order_id { get; set; }

        [Column("reservation_id")]
        public int? reservation_id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("payment_method")]
        public string payment_method { get; set; } = "GCash";

        [Required]
        [Column("amount")]
        public decimal amount { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("reference_number")]
        public string reference_number { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("customer_phone")]
        public string? customer_phone { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("payment_status")]
        public string payment_status { get; set; } = "Pending Verification";

        [Required]
        [Column("payment_date")]
        public DateTime payment_date { get; set; } = DateTime.Now;

        [Column("paymentqr_id")]
        public int? paymentqr_id { get; set; }
    }
}