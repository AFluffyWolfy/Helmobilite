using Helmobilite.Models;
using Microsoft.EntityFrameworkCore;

namespace Helmobilite.Repositories
{
    public class ClientRepository
    {
        private readonly HelmobiliteDbContext _context;
        public IEnumerable<Client> Clients { get => _context.Clients; }
        public ClientRepository(HelmobiliteDbContext context)
        {
            _context = context;
        }

        public async Task UpdateClientTrustStatus(Client clientToUpdate)
        {
            try
            {
                _context.Update(clientToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        public Client FindClientById(string id) => _context.Clients.Find(id);
        public bool ClientExists(string id)
        {
            return (_context.Clients?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Récupère tous les clients qui ont fait au moins une demande de livraison
        /// </summary>
        /// <returns>
        /// Une IEnumerable de Client contenant tous les clients avec au moins une demande de livraisons.
        /// </returns>
        public List<Client> GetAllClientWithDeliveries()
            =>  Clients
                .Where(client =>
                    _context.Deliveries.Any(delivery => delivery.IdCustomer == client.Id)
                )
                .ToList();
    }
}
