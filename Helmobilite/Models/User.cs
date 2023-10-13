using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Helmobilite.Models
{
    public abstract class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
    }

    public abstract class HelmoUser : User
    {
        [Required]
        [StringLength(7)]
        public string Matricule { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date d'anniversaire")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        [AllowNull]
        [DisplayName("Photo de profil")]
        public String PicturePath { get; set; }
    }

    public class Admin : User
    {
    }

    public class Client : User
    {
        [Required]
        [StringLength(100)]
        public string EnterpriseName { get; set; }
        [Required]
        [StringLength(500)]
        public string EnterpriseAddress { get; set; }
        [Required]
        [DisallowNull]
        [DefaultValue(false)]
        [DisplayName("Mauvais payeur ?")]
        public Boolean Untrusted { get; set; }
        [AllowNull]
        [DisplayName("Logo d'entreprise")]
        public String PicturePath { get; set; } = null;
    }

    public class Dispatcher : HelmoUser
    {
        [Required]
        public StudyLevel StudyLevel { get; set; }
    }

    public class Driver : HelmoUser
    {
        [Required]
        public DriverLicense DriverLicense { get; set; }
    }
}
