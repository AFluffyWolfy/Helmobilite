using Helmobilite.Models;
using Helmobilite.Repositories;
using Helmobilite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Helmobilite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatsController : Controller
    {
        private readonly ClientRepository _clientRepository;
        private readonly DriverRepository _driverRepository;
        private readonly HelmobiliteDbContext _context;

        public StatsController(ClientRepository clientRepository, DriverRepository driverRepository, HelmobiliteDbContext context)
        {
            _clientRepository = clientRepository;
            _driverRepository = driverRepository;
            _context = context;
        }

        // GET: Stats
        public async Task<IActionResult> Index(DateTime SearchDate, string SearchClient = "", string SearchDriver = "")
        {
            /* Récupérer toutes les livraisons qui :
             * - N'ont pas de cause d'échec (FailedCause)
             * - Possède une date de unload
             * - La date de unload est inférieur à la date d'aujourd'hui
             * - Un idDriver est assigné
            */
            var deliveries = _context.Deliveries.Where(delivery => delivery.FailedCause == null && delivery.DeliveryUnloadDateTime != null && delivery.DeliveryUnloadDateTime < DateTime.Now && delivery.IdDriver != null);

            // Cas où aucune livraisons
            if (deliveries == null || !deliveries.Any())
            {
                return emptyViewModel();
            }

            var statsViewModelList = new List<StatsViewModel>();
            if(string.IsNullOrEmpty(SearchClient) && string.IsNullOrEmpty(SearchDriver) && SearchDate == DateTime.MinValue)
            {
                // Cas où on ne fait aucune recherche
                foreach (Delivery delivery in deliveries)
                {
                    StatsViewModel statsViewModel = createViewModel(delivery, out statsViewModel);
                    statsViewModelList.Add(statsViewModel);
                }
                return View(statsViewModelList.ToList());
                // Cas où on spécifie une recherche sur le client
            } if(!string.IsNullOrEmpty(SearchClient))
            {
                deliveries = deliveries.Join(_clientRepository.Clients,
                d => d.IdCustomer,
                c => c.Id,
                (d, c) => new { Delivery = d, Client = c })
                .Where(dc => dc.Client.UserName.Contains(SearchClient))
                .Select(dc => dc.Delivery);
                // Cas où on spécifie une recherche sur le conducteur
            } if(!string.IsNullOrEmpty(SearchDriver))
            {
                deliveries = deliveries.Join(_driverRepository.Drivers,
                d => d.IdDriver,
                c => c.Id,
                (d, c) => new { Delivery = d, Driver = c })
                .Where(dc => dc.Driver.UserName.Contains(SearchDriver))
                .Select(dc => dc.Delivery);
                // Cas où on spécifie une recherche sur la date
            } if (SearchDate != DateTime.MinValue)
            {
                deliveries = deliveries.Where(d => d.DeliveryUnloadDateTime < SearchDate.AddDays(1) && d.DeliveryUnloadDateTime > SearchDate);
            }

            if (deliveries == null | !deliveries.Any())
            {
                return emptyViewModel();
            }
            foreach (Delivery delivery in deliveries)
            {
                StatsViewModel statsViewModel = createViewModel(delivery, out statsViewModel);
                statsViewModelList.Add(statsViewModel);
            }

            return View(statsViewModelList.ToList());
        }

        private IActionResult emptyViewModel()
        {
            var emptyViewModel = new StatsViewModel
            {
                ClientName = "N/A",
                Driver = "N/A",
                Date = DateTime.UnixEpoch
            };
            var emptyViewModelList = new List<StatsViewModel>
                {
                    emptyViewModel
                };
            return View(emptyViewModelList);
        }

        private StatsViewModel createViewModel(Delivery delivery, out StatsViewModel statsViewModel)
        {
            statsViewModel = new StatsViewModel();
            var client = _clientRepository.FindClientById(delivery.IdCustomer);
            statsViewModel.ClientName = client.UserName;

            var driver = _driverRepository.FindDriverById(delivery.IdDriver);
            statsViewModel.Driver = driver.UserName;

            statsViewModel.Date = (DateTime)delivery.DeliveryUnloadDateTime;
            return statsViewModel;
        }

        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchClient()
        {
            var clientUsername = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString()).Get("searchClient");
            var list = _context.Clients.Where(client => client.UserName.Contains(clientUsername)).Select(client => client.UserName).ToList();
            return Ok(list);
        }

        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchDriver()
        {
            var driverUsername = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString()).Get("searchDriver");
            var list = _context.Drivers.Where(driver => driver.UserName.Contains(driverUsername)).Select(driver => driver.UserName).ToList();
            return Ok(list);
        }
    }
}
