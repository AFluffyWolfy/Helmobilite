using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helmobilite.Models;
using System.Text.RegularExpressions;
using Helmobilite.ViewModels;
using Helmobilite.Utils;
using Helmobilite.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Helmobilite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrucksController : Controller
    {
        private readonly string TRUCK_DIRECTORY = "Truck";

        private readonly HelmobiliteDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly TruckRepository _truckRepository;

        public TrucksController(HelmobiliteDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _truckRepository = new(_context);
        }

        // GET: Trucks
        public async Task<IActionResult> Index()
        {
            return _truckRepository.Trucks != null ?
                        View(_truckRepository.Trucks.ToList()) :
                        Problem("Entity set 'HelmobiliteDbContext.Trucks'  is null.");
        }

        // GET: Trucks/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _truckRepository.Trucks == null)
            {
                return NotFound();
            }

            var truck = _truckRepository.Trucks
                .FirstOrDefault(m => m.LicensePlate == id);
            if (truck == null)
            {
                return NotFound();
            }

            return View(truck);
        }

        public IEnumerable<SelectListItem> GetTruckBrandListItem() => _truckRepository.TruckBrands.Select(brand => new SelectListItem
        {
            Value = brand.Id.ToString(),
            Text = brand.Name
        }).AsEnumerable();

        public IEnumerable<SelectListItem> GetTruckModelListItem() => _truckRepository.TruckModels.Select(model => new SelectListItem
        {
            Value = model.Id.ToString(),
            Text = model.Name
        }).AsEnumerable();

        public IEnumerable<SelectListItem> GetLicensesListItem() 
            => Enum.GetValues(typeof(DriverLicense)).Cast<DriverLicense>()
            .ToList()
            .FindAll(license => license != DriverLicense.B)
            .Select(license => new SelectListItem
        {
            Value = ((int)license).ToString(),
            Text = license.ToString()
        });

        // GET: Trucks/Create
        public IActionResult Create()
        {
            ViewBag.Error = "";
            ViewBag.Brands = GetTruckBrandListItem();
            ViewBag.Models = GetTruckModelListItem();
            return View();
        }

        // POST: Trucks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicensePlate,LicenseType,MaxWeight,Brand,Model,Picture")] TruckAddViewModel truck)
        {
            ViewBag.Error = "";
            ViewBag.Brands = GetTruckBrandListItem();
            ViewBag.Models = GetTruckModelListItem();
            if (ModelState.IsValid)
            {
                if(TruckExists(truck.LicensePlate))
                {
                    ViewBag.Error = "Le camion existe déjà dans la base de donnée";
                }
                else if(!new Regex("^[0-9]{1}-[A-Za-z]{3}-[0-9]{3}$").IsMatch(truck.LicensePlate))
                {
                    ViewBag.Error = "La plaque d'immatriculation doit respecter le format [1-AAA-111]";
                }
                else
                {
                    string truckImgPath = await FilesUtils.SavePicture(truck.Picture, TRUCK_DIRECTORY, truck.LicensePlate, _environment.WebRootPath);
                    ViewBag.Error = "Coucou";
                    Truck toSave = new()
                    {
                        LicensePlate = truck.LicensePlate,
                        LicenseType = (DriverLicense)truck.LicenseType,
                        MaxWeight = truck.MaxWeight,
                        TruckModelId = truck.Model,
                        TruckBrandId = truck.Brand,
                        PicturePath = truckImgPath
                    };
                    await _truckRepository.SaveTruck(toSave);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(truck);
        }

        // GET: Trucks/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Trucks == null)
            {
                return NotFound();
            }

            var truck = _truckRepository.FindTruckByLicensePlate(id);
            if (truck == null)
            {
                return NotFound();
            }
            ViewBag.Error = "";
            ViewBag.Brands = GetTruckBrandListItem();
            ViewBag.Models = GetTruckModelListItem();
            var toEdit = new TruckEditViewModel
            {
                LicensePlate = truck.LicensePlate,
                LicenseType = (int) truck.LicenseType,
                Brand = truck.TruckBrandId,
                Model = truck.TruckModelId,
                MaxWeight = truck.MaxWeight,
                PicturePath = truck.PicturePath
            };
            return View(toEdit);
        }

        // POST: Trucks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("LicensePlate,LicenseType,MaxWeight,Picture,PicturePath,Brand,Model")] TruckEditViewModel truck)
        {
            if (id != truck.LicensePlate)
            {
                return NotFound();
            }
            ViewBag.Error = "";
            ViewBag.Brands = GetTruckBrandListItem();
            ViewBag.Models = GetTruckModelListItem();
            if(!TruckExists(id))
            {
                ViewBag.Error = "Le camions que vous souhaitez modifier n'existe pas.";
            }
            else if(ModelState.IsValid)
            {
                try
                {
                    string truckImg = truck.Picture is null ? truck.PicturePath! : await FilesUtils.SavePicture(truck.Picture, TRUCK_DIRECTORY, truck.LicensePlate, _environment.WebRootPath);
                    Truck toSave = new()
                    {
                        LicensePlate = truck.LicensePlate,
                        LicenseType = (DriverLicense)truck.LicenseType,
                        MaxWeight = truck.MaxWeight,
                        TruckModelId = truck.Model,
                        TruckBrandId = truck.Brand,
                        PicturePath = truckImg
                    };
                    await _truckRepository.UpdateTruck(toSave);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TruckExists(truck.LicensePlate))
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
            return View(truck);
        }

        // GET: Trucks/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Trucks == null)
            {
                return NotFound();
            }

            var truck = await _context.Trucks
                .FirstOrDefaultAsync(m => m.LicensePlate == id);
            if (truck == null)
            {
                return NotFound();
            }

            return View(truck);
        }

        // POST: Trucks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Trucks == null)
            {
                return Problem("Entity set 'HelmobiliteDbContext.Trucks'  is null.");
            }
            var truck = await _context.Trucks.FindAsync(id);
            if (truck != null)
            {
                _context.Trucks.Remove(truck);
                System.IO.File.Delete(truck.PicturePath);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TruckExists(string id)
        {
          return (_context.Trucks?.Any(e => e.LicensePlate == id)).GetValueOrDefault();
        }
    }
}
