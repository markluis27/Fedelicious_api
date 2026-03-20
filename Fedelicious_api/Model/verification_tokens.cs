using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fedelicious_api.Model
{
    [Table("verification_tokens")]
    public class verification_tokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string token { get; set; }

        public int customer_id { get; set; }

        public DateTime expiry_date { get; set; }
    }
}
