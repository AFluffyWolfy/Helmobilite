using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Helmobilite.Models
{
    public class Delivery
    {
        private DateTime _deliveryUnloadExpectedDateTime;
        private string _idDriver;

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(500)]
        public string LoadAddress { get; set; }
        [Required]
        [StringLength(500)]
        public string UnloadAdress { get; set; }
        [Required]
        [StringLength(100)]
        public string Content { get; set; }
        [Required]
        public DateTime DeliveryLoadDateTime { get; set; }
        public DateTime? DeliveryUnloadDateTime { get; set; }
        [Required]
        public DateTime DeliveryUnloadExpectedDateTime {
            get => _deliveryUnloadExpectedDateTime;
            set 
            {
                if(DeliveryLoadDateTime >= value)
                {
                    throw new ArgumentException("La date de déchargement doit être plus grande que celle de chargement.");
                }
                _deliveryUnloadExpectedDateTime = value;
            }
        }
        public FailedDeliveryCause? FailedCause { get; set; }
        public string IdCustomer { get; set; }
        public string? IdTruck { get; set; }
        public string? IdDriver { get; set; }
        [Required]
        public Boolean isAccepted;
        [AllowNull]
        public string? Comment { get; set; }
        [NotMapped]
        public Boolean IsDelivered { get => DeliveryUnloadDateTime != null; }
    }

    public enum FailedDeliveryCause
    {
        Maladie,
        Accident,
        ClientAbsent
    }
}
