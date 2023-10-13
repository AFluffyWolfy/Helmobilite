using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [DisplayName("Rue")]
        public string Street { get; init; }
        [Required]
        [MaxLength(10)]
        [DisplayName("Numéro")]
        public string Number { get; init; }
        [Required]
        [MaxLength(100)]
        [DisplayName("Ville")]
        public string City { get; init; }
        [Required]
        [MaxLength(10)]
        [DisplayName("Code postal")]
        public string PostalCode { get; init; }
        [Required]
        [MaxLength(100)]
        [DisplayName("Pays")]
        public string Country { get; init; }
    }
}
