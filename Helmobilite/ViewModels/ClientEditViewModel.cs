using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.ViewModels
{
    public class ClientEditViewModel
    {
        [Required]
        public string Id { get; set; }
        [DisplayName("Prénom")]
        public string FirstName { get; set; }
        [DisplayName("Nom de famille")]
        public string LastName { get; set; }
        [DisplayName("Nom de l'entreprise")]
        public string EnterpriseName { get; set; }
        [DisplayName("Adresse de l'entreprise")]
        public string EnterpriseAddress { get; set; }
        [Required]
        [DisplayName("Mauvais payeur ?")]
        public bool Untrusted { get; set; }
    }
}
