using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fedelicious_api.Model
{
    [Table("customers")]
    public class customers
    {
        [Key]
        [Column("customer_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int customer_id { get; set; }

        [Column("full_name")]
        [Required]
        [MaxLength(255)]
        public string full_name { get; set; }

        [Column("email")]
        [Required]
        [MaxLength(255)]
        public string email { get; set; }

        [Column("address")]
        public string? address { get; set; }

        [Column("password")]
        [Required]
        [MaxLength(255)]
        public string password { get; set; }

        [Column("is_verified")]
        [JsonIgnore]
        public bool is_verified { get; set; } = false;
    }
}