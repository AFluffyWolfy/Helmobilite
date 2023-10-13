using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.ViewModels
{
    public class StatsViewModel
    {
        [DisplayName("Client")]
        public string ClientName { get; set; }
        [DisplayName("Chauffeur")]
        public string Driver { get; set; }
        [DisplayName("Date de livraison effectuée")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Date { get; set; }
    }
}
