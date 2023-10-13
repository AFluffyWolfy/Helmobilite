using Helmobilite.Models;
using Helmobilite.ViewModels;

namespace Helmobilite.Repositories
{
    public class Mapper
    {
        public static Delivery ConvertViewModelToDelivery(AddDeliveryViewModel viewModel, string connectedUserId)
        {
            return new Delivery
            {
                DeliveryLoadDateTime = viewModel.DeliveryLoadDateTime,
                Content = viewModel.Content,
                DeliveryUnloadExpectedDateTime = viewModel.DeliveryUnloadExpectedDateTime,
                LoadAddress = viewModel.LoadAddress,
                UnloadAdress = viewModel.UnloadAdress,
                isAccepted = false,
                IdCustomer = connectedUserId
            };
        }

        public static Delivery ConvertViewModelToDelivery(EditDeliveryViewModel viewModel, int deliveryId, string connectedUserId)
        {
            return new Delivery
            {
                Id = deliveryId,
                DeliveryLoadDateTime = viewModel.DeliveryLoadDateTime,
                Content = viewModel.Content,
                DeliveryUnloadExpectedDateTime = viewModel.DeliveryUnloadExpectedDateTime,
                LoadAddress = viewModel.LoadAddress,
                UnloadAdress = viewModel.UnloadAdress,
                isAccepted = false,
                IdCustomer = connectedUserId
            };
        }

        public static EditDeliveryViewModel ConvertDeliveryToEditViewModel(Delivery delivery)
        {
            return new EditDeliveryViewModel
            {
                Content = delivery.Content,
                DeliveryUnloadExpectedDateTime = delivery.DeliveryUnloadExpectedDateTime,
                DeliveryLoadDateTime = delivery.DeliveryLoadDateTime,
                Id = delivery.Id,
                LoadAddress = delivery.LoadAddress,
                UnloadAdress = delivery.UnloadAdress,
                IsAccepted = delivery.IdDriver != null
            };
        }

        public static IndexDeliveryViewModel ConvertDeliveryToIndexViewModel(Delivery delivery, bool clientTrustStatus)
        {
            return new IndexDeliveryViewModel
            {
                Content = delivery.Content,
                DeliveryLoadDateTime = delivery.DeliveryLoadDateTime.ToString("Le dd MMMM yyyy à H:mm"),
                DeliveryUnloadExpectedDateTime = delivery.DeliveryUnloadExpectedDateTime.ToString("Le dd MMMM yyyy à H:mm"),
                DeliveryUnloadDateTime = delivery.DeliveryUnloadDateTime == null ? "Pas encore livré" : delivery.DeliveryUnloadDateTime?.ToString("Le dd MMMM yyyy à H:mm"),
                Id = delivery.Id,
                LoadAddress = delivery.LoadAddress,
                UnloadAdress = delivery.UnloadAdress,
                FailedCause = delivery.FailedCause == null ? "Pas de problème" : delivery.FailedCause.ToString(),
                isClientUntrusted = clientTrustStatus,
                TruckPlate = delivery.IdTruck
            };
        }

        public static ValidateDeliveryViewModel ConvertDeliveryToValidateViewModel(Delivery delivery, string driverName)
        {
            return new ValidateDeliveryViewModel
            {
                Content = delivery.Content,
                DriverName = driverName,
                Comment = delivery.Comment
            };
        }

        public static MissedDeliveryViewModel ConvertDeliveryToMissedDeliveryViewModel(Delivery delivery, string driverName)
        {
            return new MissedDeliveryViewModel
            {
                Content = delivery.Content,
                DriverName = driverName,
                MissedCause = delivery.FailedCause
            };
        }
    }
}
