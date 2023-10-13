using Helmobilite.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.ViewModels
{
    public class AddDeliveryViewModel
    {
        [Required]
        [StringLength(500)]
        [DisplayName("Adresse de chargement")]
        public string LoadAddress { get; set; }
        [Required]
        [StringLength(500)]
        [DisplayName("Adresse de déchargement")]
        public string UnloadAdress { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Description de chargement")]
        public string Content { get; set; }
        [Required]
        [DisplayName("Date de chargement")]
        public DateTime DeliveryLoadDateTime { get; set; }
        [Required]
        [DisplayName("Date de déchargement préférée")]
        public DateTime DeliveryUnloadExpectedDateTime { get; set; }
    }

    public class EditDeliveryViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(500)]
        [DisplayName("Adresse de chargement")]
        public string LoadAddress { get; set; }
        [Required]
        [StringLength(500)]
        [DisplayName("Adresse de déchargement")]
        public string UnloadAdress { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Description de chargement")]
        public string Content { get; set; }
        [Required]
        [DisplayName("Date de chargement")]
        public DateTime DeliveryLoadDateTime { get; set; }
        [Required]
        [DisplayName("Date de déchargement préférée")]
        public DateTime DeliveryUnloadExpectedDateTime { get; set; }
        public bool IsAccepted { get; set; }
    }

    public class IndexDeliveryViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(500)]
        [DisplayName("Adresse de chargement")]
        public string LoadAddress { get; set; }
        [Required]
        [StringLength(500)]
        [DisplayName("Adresse de déchargement")]
        public string UnloadAdress { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Description de chargement")]
        public string Content { get; set; }
        [Required]
        [DisplayName("Date de chargement")]
        public string DeliveryLoadDateTime { get; set; }
        [Required]
        [DisplayName("Date de déchargement préférée")]
        public string DeliveryUnloadExpectedDateTime { get; set; }
        [Required]
        [DisplayName("Date de déchargement")]
        public string DeliveryUnloadDateTime { get; set; }
        [DisplayName("Status de la livraison")]
        public string Status { get => DeliveryUnloadDateTime != null ? "Livraison terminée" : TruckPlate != null ? "En cours d'achiminement" : "En attente d'acceptation"; }
        [DisplayName("Camion")]
        public string? TruckPlate { get; set; }
        [DisplayName("Cause de non déchargement")]
        public string FailedCause { get; set; }
        public bool isClientUntrusted { get; set; }
        public bool IsAccepted { get => TruckPlate != null; }
    }

    public class AttributeDeliveryViewModel
    {
        [DisplayName("Camion")]
        public string? TruckPlate { get; set; }
        [DisplayName("Chauffeur")]
        public string? Driver { get; set; }
    }

    public class ValidateDeliveryViewModel
    {
        [DisplayName("Chauffeur en charge")]
        public string DriverName { get; set; }
        [DisplayName("Commentaire(falcultatif)")]
        [MaxLength(250)]
        public string? Comment { get; set; }
        [DisplayName("Description du chargement")]
        public string Content { get; set; }
    }

    public class MissedDeliveryViewModel
    {
        [DisplayName("Chauffeur en charge")]
        public string DriverName { get; set; }
        [DisplayName("Raison du manquement")]
        public FailedDeliveryCause? MissedCause { get; set; }
        [DisplayName("Description du chargement")]
        public string Content { get; set; }
    }
}
