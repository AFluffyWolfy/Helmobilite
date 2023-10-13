using Helmobilite.Models;
using Microsoft.EntityFrameworkCore;

namespace Helmobilite.Repositories
{
    public class DriverRepository
    {
        private readonly HelmobiliteDbContext _context;
        public IEnumerable<Driver> Drivers { get => _context.Drivers; }
        public DriverRepository(HelmobiliteDbContext context)
        {
            _context = context;
        }

        public async Task UpdateDriverLicense(Driver driverToUpdate)
        {
            try
            {
                _context.Update(driverToUpdate);
                await _context.SaveChangesAsync();

                if(driverToUpdate.DriverLicense == DriverLicense.None || driverToUpdate.DriverLicense == DriverLicense.B) {
                    IEnumerable<Delivery> deliveries = await _context.Deliveries.Where(delivery => delivery.IdDriver == driverToUpdate.Id && delivery.DeliveryUnloadDateTime == null).ToListAsync();
                    foreach (Delivery delivery in deliveries)
                    {
                        delivery.IdDriver = null;
                        delivery.IdTruck = null;
                    }
                    if (deliveries != null) await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        public Driver FindDriverById(string id) => _context.Drivers.Find(id);
        public bool DriverExists(string id)
        {
            return (_context.Drivers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
