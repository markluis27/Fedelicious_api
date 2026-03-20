using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("reservations")]
    public class reservations
    {
        [Key]
        [Column("reservation_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int reservation_id { get; set; }

        [Required]
        [Column("customer_id")]
        public int customer_id { get; set; }

        // Nullable int? is correct para sa admin approval later
        [Column("confirmed_by_admin_id")]
        public int? confirmed_by_admin_id { get; set; }

        [Required]
        [Column("reservation_date")]
        public DateTime reservation_date { get; set; }

        // Stored as SQL TIME — use TimeSpan to match ADO.NET mapping
        [Column("reservation_time")]
        public TimeSpan? reservation_time { get; set; }

        [Required]
        [Column("number_of_guests")]
        public int number_of_guests { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("reservation_status")]
        public string reservation_status { get; set; } = "Pending";

        [Column("notes")]
        public string? notes { get; set; }

        [Column("downpayment_amount")]
        public decimal? downpayment_amount { get; set; }
    }
}