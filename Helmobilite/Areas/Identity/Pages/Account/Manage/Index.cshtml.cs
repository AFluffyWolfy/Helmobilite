// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Helmobilite.Models;
using Helmobilite.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Dispatcher = Helmobilite.Models.Dispatcher;


namespace Helmobilite.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// Toutes les données sont dans un ViewModel (le InputModel) pour éviter de casser
    /// tout l'automatise apporter par Identity. Nous sommes au courant qu'il faudrait
    /// séparer en plusieurs ViewModel mais c'est un choix pour avoir une stabilité avec Identity
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [DataType(DataType.Text)]
            [Display(Name = "Nom de famille")]
            [MaxLength(50)]
            public string LastName { get; set; }
            [DataType(DataType.Text)]
            [Display(Name = "Prénom")]
            [MaxLength(50)]
            public string FirstName { get; set; }
            [MaxLength(7)]
            [Display(Name = "Matricule")]
            public string Matricule { get; set; }
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Nom de l'entreprise")]
            public string EnterpriseName { get; set; }
            [StringLength(500)]
            [DataType(DataType.Text)]
            [Display(Name = "Addresse de l'entreprise")]
            public string EnterpriseAddress { get; set; }
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            [Display(Name = "Date d'anniversaire")]
            [DataType(DataType.Date)]
            public DateTime Birthday { get; set; }
            [Display(Name = "Permis le plus haut")]
            public DriverLicense DriverLicense { get; set; }
            [Display(Name = "Niveau d'étude")]
            public StudyLevel StudyLevel { get; set; }
            [DisplayName("Changer la photo")]
            [DataType(DataType.Upload)]
            public IFormFile? Picture { get; set; }
            [DisplayName("Ancienne photo")]
            public string? PicturePath { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;

            if (user is Client)
            {
                var client = (Client)user;
                Input = new InputModel
                {
                    LastName = client.LastName,
                    FirstName = client.FirstName,
                    EnterpriseAddress = client.EnterpriseAddress,
                    EnterpriseName = client.EnterpriseName,
                    PicturePath = client.PicturePath
                };
            } else if (user is Dispatcher)
            {
                var dispatcher = (Dispatcher)user;
                Input = new InputModel
                {
                    LastName = dispatcher.LastName,
                    FirstName = dispatcher.FirstName,
                    Matricule = dispatcher.Matricule,
                    Birthday = dispatcher.Birthday,
                    StudyLevel = dispatcher.StudyLevel,
                    PicturePath = dispatcher.PicturePath
                };
            } else if (user is Driver)
            {
                var driver = (Driver)user;
                Input = new InputModel
                {
                    LastName = driver.LastName,
                    FirstName = driver.FirstName,
                    Matricule = driver.Matricule,
                    Birthday = driver.Birthday,
                    DriverLicense = driver.DriverLicense,
                    PicturePath= driver.PicturePath
                };
            } else if (user is Admin)
            {
                var admin = (Admin)user;
                Input = new InputModel
                {
                    LastName = admin.LastName,
                    FirstName = admin.FirstName,
                };
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                await LoadAsync(user);
                ViewData["Error"] = "Erreur inconnue dans le formulaire, vérifiez vos champs";
                return Page();
            }

            if(user is Client)
            {
                var client = (Client)user;
                if (Input.EnterpriseAddress != client.EnterpriseAddress && !String.IsNullOrEmpty(client.EnterpriseAddress))
                {
                    client.EnterpriseAddress = Input.EnterpriseAddress;
                }
                if (Input.EnterpriseName != client.EnterpriseName && !String.IsNullOrEmpty(client.EnterpriseAddress))
                {
                    client.EnterpriseName = Input.EnterpriseName;
                }
                if(Input.Picture != null)
                {
                    string logoImgPath = await SaveFileClient(Input.Picture, client);
                    client.PicturePath = logoImgPath;
                }
                
            } else if (user is Dispatcher)
            {
                var dispatcher = (Dispatcher)user;
                if(Input.Birthday >= DateTime.Now)
                {
                    ViewData["Error"] = "Non, vous n'êtes pas né dans le futur ! >:(";
                    await LoadAsync(user);
                    return Page();
                }
                else if (Input.Birthday != dispatcher.Birthday)
                {
                    dispatcher.Birthday = Input.Birthday;
                }
                if (Input.Picture != null)
                {
                    string profilePicImgPath = await SaveFileHelmoUsers(Input.Picture, dispatcher);
                    dispatcher.PicturePath = profilePicImgPath;
                }
            } else if (user is Driver)
            {
                var driver = (Driver)user;
                if (Input.Birthday >= DateTime.Now)
                {
                    ViewData["Error"] = "Non, vous n'êtes pas né dans le futur ! >:(";
                    await LoadAsync(user);
                    return Page();
                }
                if (Input.Birthday != driver.Birthday)
                {
                    driver.Birthday = Input.Birthday;
                }
                if (Input.Picture != null)
                {
                    string profilePicImgPath = await SaveFileHelmoUsers(Input.Picture, driver);
                    driver.PicturePath = profilePicImgPath;
                }
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Votre profil a été mis à jour";
            return RedirectToPage();
        }

        private async Task<string> SaveFileClient(IFormFile pictureFile, Client client)
        {
            var imgPath = Path.Combine("XXXX", "Img", "ClientLogo");
            try
            {
                var webRootPath = _environment.WebRootPath;
                var fileName = GetPath("ClientLogo", $"{client.FirstName}-{client.LastName}-{client.EnterpriseName}" + ".jpg");
                var filePath = Path.Combine(webRootPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await pictureFile.CopyToAsync(fileStream);
                }
                return Path.Combine("/", fileName);
            }
            catch (IOException copyError)
            {
                Console.WriteLine(copyError.Message);
            }

            return imgPath;
        }
        private async Task<string> SaveFileHelmoUsers(IFormFile pictureFile, HelmoUser helmoUser)
        {
            var imgPath = Path.Combine("XXXX", "Img", "HelmoProfilePictures");
            try
            {
                var webRootPath = _environment.WebRootPath;
                var fileName = GetPath("HelmoProfilePictures", $"{helmoUser.Matricule}-{helmoUser.FirstName}-{helmoUser.LastName}" + ".jpg");
                var filePath = Path.Combine(webRootPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await pictureFile.CopyToAsync(fileStream);
                }
                return Path.Combine("/", fileName);
            }
            catch (IOException copyError)
            {
                Console.WriteLine(copyError.Message);
            }

            return imgPath;
        }
        private static string GetPath(string dir, string picture)
        {
            return Path.Combine("XXXX", "Img", dir, picture);
        }
    }
}
