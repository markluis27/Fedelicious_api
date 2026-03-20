using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("delivery_locations")]
    public class delivery_locations
    {
        [Key]
        [Column("location_id")]
        public int location_id { get; set; }

        [Column("location_name")]
        [Required]
        [MaxLength(255)]
        public string location_name { get; set; }

        [Column("delivery_fee")]
        [Required]
        public decimal delivery_fee { get; set; }

    }
}
