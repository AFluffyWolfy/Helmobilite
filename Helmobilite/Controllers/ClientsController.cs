using Helmobilite.Models;
using Helmobilite.Repositories;
using Helmobilite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmobilite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClientsController : Controller
    {
        private readonly HelmobiliteDbContext _context;
        private readonly ClientRepository _clientRepository;
        public ClientsController(HelmobiliteDbContext context)
        {
            _context = context;
            _clientRepository = new(_context);
        }
        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return _clientRepository.Clients != null ?
                        View(_clientRepository.Clients.ToList()) :
                        Problem("Entity set 'HelmobiliteDbContext.Drivers'  is null.");
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var client = _clientRepository.FindClientById(id);
            if (client == null)
            {
                return NotFound();
            }
            ViewBag.Error = "";
            var toEdit = new ClientEditViewModel
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                EnterpriseName = client.EnterpriseName,
                EnterpriseAddress = client.EnterpriseAddress,
                Untrusted = client.Untrusted
            };
            return View(toEdit);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,EnterpriseName,EnterpriseAddress,Untrusted")] ClientEditViewModel client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }
            ViewBag.Error = "";
            if (!_clientRepository.ClientExists(id))
            {
                ViewBag.Error = "Le client que vous souhaitez modifier n'existe pas.";
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    Client toSave = _clientRepository.FindClientById(id);
                    toSave.Untrusted = client.Untrusted;
                    await _clientRepository.UpdateClientTrustStatus(toSave);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_clientRepository.ClientExists(client.Id))
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
            return View(client);
        }
    }
}
