using Helmobilite.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.ViewModels
{
    public class DriverEditViewModel
    {
        [Required]
        public string Id { get; set; }
        [DisplayName("Prénom")]
        public string FirstName { get; set; }
        [DisplayName("Nom de famille")]
        public string LastName { get; set; }
        [DisplayName("Matricule")]
        public string Matricule { get; set; }
        [Required]
        [DisplayName("Permis le plus haut")]
        public int DriverLicense { get; set; }
    }
}
