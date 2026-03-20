using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fedelicous_api.Model
{
    [Table("categories")]
    public class categories
    {
        [Key]
        [Column("category_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int category_id { get; set; }

        [Column("category_name")]
        [Required]
        [MaxLength(255)]
        public string category_name { get; set; }
    }
}
