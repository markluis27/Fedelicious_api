using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("admins")]
    public class admins
    {
        [Key]
        [Column("admin_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int admin_id { get; set; }

        [Column("full_name")]
        [Required]
        [MaxLength(255)]
        public string full_name { get; set; }

        [Column("username")]
        [Required]
        [MaxLength(255)]
        public string username { get; set; }

        [Column("password")]
        [Required]
        [MaxLength(255)]
        public string password { get; set; }
        [Column("role")]
        public string role { get; set; }

    }
}
