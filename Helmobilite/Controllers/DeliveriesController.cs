using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helmobilite.Models;
using Microsoft.AspNetCore.Authorization;
using Helmobilite.ViewModels;
using System.Security.Claims;
using Helmobilite.Repositories;

namespace Helmobilite.Controllers
{
    public class DeliveriesController : Controller
    {
        private readonly DeliveryRepository _deliveryRepository;
        private readonly ClientRepository _clientRepository;
        private readonly TruckRepository _truckRepository;
        private readonly UserRepository _userRepository;
        private readonly HelmobiliteDbContext _context;

        public DeliveriesController(HelmobiliteDbContext context, ClientRepository clientRepository, TruckRepository truckRepository, DeliveryRepository deliveryRepository, UserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _truckRepository = truckRepository;
            _deliveryRepository = deliveryRepository;
            _clientRepository = clientRepository;
        }

        // GET: Deliveries
        [Authorize(Roles = "Dispatcher, Client, Driver, Admin")]
        public async Task<IActionResult> Index()
        {
            if (_context.Deliveries != null)
            {
                Problem("Entity set 'HelmobiliteDbContext.Deliveries'  is null.");
            }
            ViewBag.Role = GetConnectedUserRole();
            if(User.IsInRole("Admin"))
            {
                ViewBag.Clients = _clientRepository.GetAllClientWithDeliveries();
            }
            return View(_deliveryRepository.GetDeliveriesForUser(GetConnectedUser(), GetConnectedUserRole()).AsEnumerable());
        }

        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Deliveries == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Deliveries/Create
        [Authorize(Roles = "Client")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoadAddress,UnloadAdress,Content,DeliveryLoadDateTime,DeliveryUnloadExpectedDateTime")] AddDeliveryViewModel delivery)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _deliveryRepository.AddDelivery(Mapper.ConvertViewModelToDelivery(delivery, GetConnectedUser()));
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(ArgumentException e)
            {
                ViewData["Error"] = e.Message;
            }
            return View(delivery);
        }

        // GET: Deliveries/Edit/5
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Deliveries == null)
            {
                return NotFound();
            }
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }
            return View(Mapper.ConvertDeliveryToEditViewModel(delivery));
        }

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LoadAddress,UnloadAdress,Content,DeliveryLoadDateTime,DeliveryUnloadExpectedDateTime")] EditDeliveryViewModel delivery)
        {
            if (id != delivery.Id)
            {
                return NotFound();
            }
            var deliveryFound = await _context.Deliveries.FindAsync(id);
            if (deliveryFound == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _deliveryRepository.UpdateDelivery(Mapper.ConvertViewModelToDelivery(delivery, id, GetConnectedUser()));
                }
                catch (ArgumentException e)
                {
                    ViewData["Error"] = e.Message;
                    return View(delivery);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(delivery);
        }

        private string GetConnectedUserRole() => ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role)?.Value;

        private string GetConnectedUser() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [Authorize(Roles = "Dispatcher")]
        public async Task<IActionResult> Attribute(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Delivery delivery = _deliveryRepository.GetDeliveryById((int)id);
            ViewBag.TruckPlates = GetFreeTruckSelectItemsForDelivery(delivery);
            ViewBag.Drivers = GetFreeDriversSelectItemsForDelivery(delivery);
            AttributeDeliveryViewModel viewModel = new()
            {
                Driver = delivery.IdDriver,
                TruckPlate = delivery.IdTruck
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dispatcher")]
        public async Task<IActionResult> Attribute(int? id, [Bind("Driver,TruckPlate")] AttributeDeliveryViewModel deliveryToAttribue)
        {
            Delivery deliveryForId = _deliveryRepository.GetDeliveryById((int)id);
            ViewBag.TruckPlates = GetFreeTruckSelectItemsForDelivery(deliveryForId);
            ViewBag.Drivers = GetFreeDriversSelectItemsForDelivery(deliveryForId);
            if (ModelState.IsValid)
            {
                Driver driverForDelivery = _userRepository.GetDriverById(deliveryToAttribue.Driver);
                Truck truckForDelivery = _truckRepository.FindTruckByLicensePlate(deliveryToAttribue.TruckPlate);
                if (driverForDelivery == null)
                {
                    ViewBag.Error = "Le chauffeur sélectionné n'existe pas.";
                    return View(deliveryToAttribue);
                }
                if (truckForDelivery == null)
                {
                    ViewBag.Error = "Le camion sélectionné n'existe pas.";
                    return View(deliveryToAttribue);
                }
                if (truckForDelivery.LicenseType > driverForDelivery.DriverLicense)
                {
                    ViewBag.Error = "Un chauffeur ne peux pas conduire un camion demandant une license plus élevé que la sienne";
                    return View(deliveryToAttribue);
                }
                var deliveryToEdit = _context.Deliveries.Where(d => d.Id == id).FirstOrDefault();
                deliveryToEdit.IdDriver = deliveryToAttribue.Driver;
                deliveryToEdit.IdTruck = deliveryToAttribue.TruckPlate;
                _deliveryRepository.UpdateDelivery(deliveryToEdit);
            }
            return RedirectToAction(nameof(Index)); ;
        }

        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Deliveries == null)
            {
                return NotFound();
            }

            var delivery = _deliveryRepository.GetDeliveryById((int)id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Deliveries == null)
            {
                return Problem("Entity set 'HelmobiliteDbContext.Deliveries'  is null.");
            }
            var delivery = _deliveryRepository.GetDeliveryById((int)id);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Validate(int id)
        {
            if (_context.Deliveries == null)
            {
                return Problem("Entity set 'HelmobiliteDbContext.Deliveries'  is null.");
            }
            var delivery = await _context.Deliveries.FindAsync(id);
            var driver = _userRepository.GetDriverById(delivery.IdDriver);
            return View(Mapper.ConvertDeliveryToValidateViewModel(
                delivery,
                $"{driver.LastName} {driver.FirstName}"));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ClientDeliveries(string idClient)
        {
            Client client = _clientRepository.FindClientById(idClient);
            ViewBag.ClientAdress = client.EnterpriseAddress;
            ViewBag.ClientName = $"{client.LastName} {client.FirstName}";
            ViewBag.ClientEntrepiseName = client.EnterpriseName;
            return View(_deliveryRepository.GetDeliveriesForClient(idClient));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Validate(int id, [Bind("Comment")] ValidateDeliveryViewModel deliveryToValidate)
        {
            Delivery deliveryToComment = _deliveryRepository.GetDeliveryById(id);
            if (deliveryToComment == null)
            {
                return NotFound();
            }
            deliveryToComment.Comment = deliveryToValidate.Comment;
            deliveryToComment.DeliveryUnloadDateTime = DateTime.Now;
            _deliveryRepository.UpdateDelivery(deliveryToComment);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Missed(int id)
        {
            if (_context.Deliveries == null)
            {
                return Problem("Entity set 'HelmobiliteDbContext.Deliveries'  is null.");
            }
            var delivery = await _context.Deliveries.FindAsync(id);
            var driver = _userRepository.GetDriverById(delivery.IdDriver);
            return View(Mapper.ConvertDeliveryToMissedDeliveryViewModel(
                delivery,
                $"{driver.LastName} {driver.FirstName}"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Missed(int id, [Bind("MissedCause")] MissedDeliveryViewModel deliveryToValidate)
        {
            Delivery deliveryFailed = _deliveryRepository.GetDeliveryById(id); 
            if (deliveryFailed == null)
            {
                return NotFound();
            }
            deliveryFailed.FailedCause = deliveryToValidate.MissedCause;
            _deliveryRepository.UpdateDelivery(deliveryFailed);
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryExists(int id)
        {
            return (_context.Deliveries?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // Useful methods

        /// <summary>
        /// Retourne un IEnumerable de SelectListItem avec tous les chauffeurs disponible pour une livraison donnée.
        /// </summary>
        /// <param name="delivery">Liraison</param>
        /// <returns>
        /// IEnumerable de SelectListItem avec en Value l'id du chauffeur et en Text son prénom et son nom
        /// </returns>
        private IEnumerable<SelectListItem> GetFreeDriversSelectItemsForDelivery(Delivery delivery)
            => _userRepository.GetFreeDriversForPeriod(delivery.DeliveryLoadDateTime, delivery.DeliveryUnloadExpectedDateTime, delivery.Id).Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FirstName} {d.LastName}"
            }).AsEnumerable();

        /// <summary>
        /// Retourne un IEnumerable de SelectListItem avec tous les camions disponible pour une livraison donnée.
        /// </summary>
        /// <param name="delivery">Liraison</param>
        /// <returns>
        /// IEnumerable de SelectListItem avec en Value et en Text la plaque d'immatricule d'un camion.
        /// </returns>
        private IEnumerable<SelectListItem> GetFreeTruckSelectItemsForDelivery(Delivery delivery)
            => _truckRepository.GetPlateOfFreeTruckForPeriod(delivery.DeliveryLoadDateTime, delivery.DeliveryUnloadExpectedDateTime, delivery.Id).Select(d => new SelectListItem
            {
                Value = d,
                Text = d
            }).AsEnumerable();
    }
}
