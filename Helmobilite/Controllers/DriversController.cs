using Helmobilite.Models;
using Helmobilite.Repositories;
using Helmobilite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Helmobilite.ViewModels.DriverEditViewModel;

namespace Helmobilite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DriversController : Controller
    {
        private readonly HelmobiliteDbContext _context;
        private readonly DriverRepository _driverRepository;
        public DriversController(HelmobiliteDbContext context)
        {
            _context = context;
            _driverRepository = new(_context);
        }
        // GET: Driver
        public async Task<IActionResult> Index()
        {
            return _driverRepository.Drivers != null ?
                        View(_driverRepository.Drivers.ToList()) :
                        Problem("Entity set 'HelmobiliteDbContext.Drivers'  is null.");
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = _driverRepository.FindDriverById(id);
            if (driver == null)
            {
                return NotFound();
            }
            ViewBag.Error = "";
            var toEdit = new DriverEditViewModel
            {
                Id = driver.Id,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                Matricule = driver.Matricule,
                DriverLicense = (int)driver.DriverLicense
            };
            return View(toEdit);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Matricule,DriverLicense")] DriverEditViewModel driver)
        {
            if (id != driver.Id)
            {
                return NotFound();
            }
            ViewBag.Error = "";
            if (!_driverRepository.DriverExists(id))
            {
                ViewBag.Error = "Le conducteur que vous souhaitez modifier n'existe pas.";
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    Driver toSave = _driverRepository.FindDriverById(id);
                    toSave.DriverLicense = (DriverLicense)driver.DriverLicense;
                    await _driverRepository.UpdateDriverLicense(toSave);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_driverRepository.DriverExists(driver.Id))
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
            return View(driver);
        }
    }
}
