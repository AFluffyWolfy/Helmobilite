using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.ViewModels
{
    public class TruckViewModel
    {
        public string LicensePlate { get; init; }
        public IFormFile? Picture { get; set; }
    }
    public class TruckAddViewModel : TruckViewModel
    {
        [Required]
        [MaxLength(9)]
        [MinLength(9)]
        [DisplayName("Plaque d'immatriculation")]
        public string LicensePlate
        {
            get => base.LicensePlate;
            init => base.LicensePlate = value;
        }
        [Required]
        [DefaultValue(0)]
        [DisplayName("Marque")]
        public int Brand { get; set; }
        [Required]
        [DefaultValue(0)]
        [DisplayName("Modèle")]
        public int Model { get; set; }
        [Required]
        [DefaultValue(0)]
        [DisplayName("License requise")]
        public int LicenseType { get; set; }
        [Required]
        [DefaultValue(0)]
        [DisplayName("Charge maximale")]
        public int MaxWeight { get; set; }
        [Required]
        [DisplayName("Photo")]
        public IFormFile? Picture
        {
            get => base.Picture;
            set
            {
                base.Picture = value;
            }
        }
    }

    public class TruckEditViewModel : TruckViewModel
    {
        [Required]
        [MaxLength(9)]
        [MinLength(9)]
        [DisplayName("Plaque d'immatriculation")]
        public string LicensePlate
        {
            get => base.LicensePlate;
            init => base.LicensePlate = value;
        }
        [Required]
        [DefaultValue(0)]
        [DisplayName("Marque")]
        public int Brand { get; set; }
        [Required]
        [DefaultValue(0)]
        [DisplayName("Modèle")]
        public int Model { get; set; }
        [Required]
        [DefaultValue(0)]
        [DisplayName("License requise")]
        public int LicenseType { get; set; }
        [Required]
        [DefaultValue(0)]
        [DisplayName("Charge maximale")]
        public int MaxWeight { get; set; }
        [DisplayName("Changer la photo")]
        public IFormFile? Picture
        {
            get => base.Picture;
            set
            {
                base.Picture = value;
            }
        }
        [DisplayName("Ancienne photo")]
        public string? PicturePath { get; set; }
    }
}
