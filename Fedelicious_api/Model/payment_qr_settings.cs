using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("payment_qr_settings")]
    public class payment_qr_settings
    {
        [Key]
        [Column("paymentqr_id")]
        public int paymentqr_id { get; set; }

        [Required]
        [Column("qr_name")]
        [MaxLength(100)]
        public string qr_name { get; set; }

        [Required]
        [Column("qr_accname")]
        [MaxLength(150)]
        public string qr_accname { get; set; }

        [Required]
        [Column("qr_image")]
        public byte[] qr_image { get; set; }

        [Column("is_active")]
        public bool is_active { get; set; }

        [Column("updated_at")]
        public DateTime updated_at { get; set; }
    }
}