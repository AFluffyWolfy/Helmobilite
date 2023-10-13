using Helmobilite.Models;
using Helmobilite.ViewModels;

namespace Helmobilite.Repositories
{
    public class DeliveryRepository
    {
        private readonly HelmobiliteDbContext _context;

        public DeliveryRepository(HelmobiliteDbContext context)
        {
            _context = context;
        }

        public Delivery GetDeliveryById(int id)
        {
            return _context.Deliveries.Where(delivery => delivery.Id == id).First();
        }

        public IEnumerable<IndexDeliveryViewModel> GetDeliveriesForClient(string idClient)
            => _context.Deliveries
                .Where(delivery => delivery.IdCustomer == idClient)
                .Select(delivery => Mapper.ConvertDeliveryToIndexViewModel(delivery, false))
                .ToList();

        public List<IndexDeliveryViewModel> GetDeliveriesForUser(string idUser, string role)
        {
            var allDeliveries = GetDeliveryForUserWithRole(idUser, role);
            var deliveries = new List<IndexDeliveryViewModel>();
            foreach (var d in allDeliveries)
            {
                var clientTrustStatus = _context.Clients.Find(d.IdCustomer).Untrusted;
                deliveries.Add(Mapper.ConvertDeliveryToIndexViewModel(d, clientTrustStatus));
            }
            return deliveries;
        }

        public void AddDelivery(Delivery toAdd)
        {
            var existingEntity = _context.Set<Delivery>().Local.FirstOrDefault(e => e.Id == toAdd.Id);

            if (existingEntity == null)
            {
                _context.Add(toAdd);
            }
            else
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(toAdd);
            }

            _context.SaveChanges();
        }

        public void UpdateDelivery(Delivery toAdd)
        {
            var existingEntity = _context.Set<Delivery>().Local.FirstOrDefault(e => e.Id == toAdd.Id);

            if (existingEntity == null)
            {
                _context.Update(toAdd);
            }
            else
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(toAdd);
            }

            _context.SaveChanges();
        }

        private IEnumerable<Delivery> GetDeliveryForUserWithRole(string idUser, string role)
        {
            switch(role)
            {
                case "Dispatcher":
                case "Admin":
                    return _context.Deliveries.ToList();
                case "Driver":
                    return _context.Deliveries.Where(del => del.IdDriver != null && del.IdDriver.Equals(idUser)).ToList();
                case "Client":
                    return _context.Deliveries.Where(del => del.IdCustomer.Equals(idUser)).ToList();
                default:
                    return new List<Delivery>();
            }
        }
    }
}
