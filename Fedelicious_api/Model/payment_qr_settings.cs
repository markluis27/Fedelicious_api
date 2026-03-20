using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("payment_qr_settings")]
    public class payment_qr_settings
    {
        [Key]
        [Column("paymentqr_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int paymentqr_id { get; set; }

        [Column("qr_name")]
        [Required]
        public string qr_name { get; set; }

        [Column("qr_image")]
        [Required]
        public byte[] qr_image { get; set; }

        [Column("is_active")]
        public bool is_active { get; set; }

        [Column("updated_at")]
        public DateTime updated_at { get; set; }
    }
}