using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fedelicous_api.Model;

namespace Fedelicious_api.Model
{
    [Table("menu_items")]
    public class menu_items
    {
        [Key]
        [Column("menu_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int menu_id { get; set; }

        [Column("category_id")]
        [Required]
        public int category_id { get; set; }

        [Column("food_name")]
        [Required]
        [MaxLength(255)]
        public string food_name { get; set; }

        [Column("description")]
        public string description { get; set; }

        [Column("price")]
        [Required]
        public decimal price { get; set; }

        [Column("image")]
        public string image { get; set; }

        
    }
}
