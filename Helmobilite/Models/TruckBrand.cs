using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Models
{
    public class TruckBrand
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
