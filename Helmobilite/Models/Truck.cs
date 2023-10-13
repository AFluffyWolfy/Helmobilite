using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Helmobilite.Models
{
    public class Truck
    {
        [Key]
        [StringLength(9)]
        [DisplayName("Plaque d'immatriculation")]
        public string LicensePlate { get; set; }
        [Required]
        [StringLength(50)]
        [ForeignKey("TruckBrand")]
        [DisplayName("Marque")]
        public int TruckBrandId { get; set; }
        [AllowNull]
        public TruckBrand TruckBrand { get; set; }
        [Required]
        [StringLength(50)]
        [ForeignKey("TruckModel")]
        [DisplayName("Modèle")]
        public int TruckModelId { get; set; }
        [AllowNull]
        public TruckModel TruckModel { get; set; }
        [Required]
        [DisplayName("License requise")]
        public DriverLicense LicenseType { get; set; }
        [Required]
        [DisplayName("Charge maximale")]
        public int MaxWeight { get; set; }
        [AllowNull]
        [DisplayName("Photo")]
        public String PicturePath { get; set; }
    }
}
