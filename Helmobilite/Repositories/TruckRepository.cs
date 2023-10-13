using Helmobilite.Models;
using Helmobilite.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Helmobilite.Repositories
{
    public class TruckRepository
    {
        private readonly HelmobiliteDbContext _context;

        public IEnumerable<TruckBrand> TruckBrands { get => _context.TruckBrand; }
        public IEnumerable<TruckModel> TruckModels { get => _context.TruckModel; }
        public IEnumerable<Truck> Trucks { get => _context.Trucks; }

        public TruckRepository(HelmobiliteDbContext context)
        {
            _context = context;
        }

        public async Task UpdateTruck(Truck truckToSave)
        {
            try
            {
                _context.Update(truckToSave);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task SaveTruck(Truck truckToSave)
        {
            try
            {
                _context.Add(truckToSave);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// Récupère un camion à partir d'un plaque d'immatriculation donnée.
        /// </summary>
        /// <param name="licensePlate">Plaque d'immatriculation du camion recherché</param>
        /// <returns>
        /// Null si le camion n'existe pas, sinon un objet Truck contenant toutes les informations du camion recherché.
        /// </returns>
        public Truck FindTruckByLicensePlate(string licensePlate) => _context.Trucks.Find(licensePlate);

        /// <summary>
        /// Vérfie si un camion, avec une plaque d'immatriculation donnée, existe déjà en bd.
        /// Pour cette vérification on part du principe que si la place d'immatriculation donnée existe en bd, ça veut dire que le camion existe.
        /// </summary>
        /// <param name="licensePlate">Plaque d'immatriculation du camion recherché</param>
        /// <returns>
        /// True si le plaque se trouve en bd sinon false.
        /// </returns>
        public bool TruckExists(string licensePlate)
        {
            return (_context.Trucks?.Any(e => e.LicensePlate == licensePlate)).GetValueOrDefault();
        }

        /// <summary>
        /// Retourne un IEnumerable de string avec tous les camion disponible pour une période donnée.
        /// </summary>
        /// <param name="periodStartTime">Liraison</param>
        /// <param name="periodEndTime">Liraison</param>
        /// <param name="deliveryId">Liraison</param>
        /// <returns>
        /// IEnumerable de SelectListItem avec en Value l'id du chauffeur et en Text son prénom et son nom
        /// </returns>
        public IEnumerable<string> GetPlateOfFreeTruckForPeriod(DateTime periodStartTime, DateTime periodEndTime, int deliveryId)
        {
            var freeTrucks = _context.Trucks
                .Where(truck => _context.Deliveries
                    .Count(delivery => delivery.IdTruck == truck.LicensePlate &&
                        (delivery.DeliveryLoadDateTime >= periodStartTime || delivery.DeliveryUnloadExpectedDateTime >= periodStartTime) &&
                        (delivery.DeliveryLoadDateTime <= periodEndTime || delivery.DeliveryUnloadExpectedDateTime <= periodEndTime)
                    ) == 0
                );
            var ancientTruck = _context.Trucks.FirstOrDefault(truck => _context.Deliveries.Count(
                delivery => delivery.Id == deliveryId && delivery.IdTruck == truck.LicensePlate
            ) == 1);
            return (ancientTruck == null ? freeTrucks.ToList() : freeTrucks.ToList().Append(ancientTruck)).Select(truck => truck.LicensePlate);
        }
    }
}
