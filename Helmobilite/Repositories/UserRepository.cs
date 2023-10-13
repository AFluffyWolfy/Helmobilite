using Helmobilite.Models;
using Microsoft.EntityFrameworkCore;

namespace Helmobilite.Repositories
{
    public class UserRepository
    {
        private readonly HelmobiliteDbContext _context;

        public IEnumerable<Driver> Drivers { get => _context.Drivers; }

        public UserRepository(HelmobiliteDbContext context)
        {
            _context = context;
        }

        public Driver? GetDriverById(string id)
        {
            return Drivers.FirstOrDefault(driver => driver.Id == id);
        }

        public IEnumerable<Driver> GetFreeDriversForPeriod(DateTime start, DateTime end, int deliveryId)
        {
            var drivers = _context.Drivers
                .Where(driver => _context.Deliveries
                    .Count(delivery => delivery.IdDriver == driver.Id &&
                        (delivery.DeliveryLoadDateTime >= start.AddHours(1) || delivery.DeliveryUnloadExpectedDateTime >= start.AddHours(-1)) &&
                        (delivery.DeliveryLoadDateTime <= end.AddHours(-1) || delivery.DeliveryUnloadExpectedDateTime <= end.AddHours(1))
                    ) == 0
                );
            var ancientDriver = _context.Drivers.FirstOrDefault(driver => _context.Deliveries.Count(
                delivery => delivery.Id == deliveryId && delivery.IdDriver == driver.Id
            ) == 1);
            return ancientDriver == null ? drivers.ToList() : drivers.ToList().Append(ancientDriver);
        }

    }
}
