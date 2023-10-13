// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Helmobilite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.RegularExpressions;

namespace Helmobilite.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Toutes les données sont dans un ViewModel (le InputModel) pour éviter de casser
    /// tout l'automatise apporter par Identity. Nous sommes au courant qu'il faudrait
    /// séparer en plusieurs ViewModel mais c'est un choix pour avoir une stabilité avec Identity
    /// </summary>
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            public string role { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [EmailAddress]
            [Display(Name = "Email")]
            [StringLength(256, ErrorMessage = "Le champ Email est requis.", MinimumLength = 1)]
            public string Email { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(50)]
            [Display(Name = "Nom de famille")]
            [StringLength(50, ErrorMessage = "Le champ Nom de famille est requis.", MinimumLength = 1)]
            public string LastName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(50)]
            [Display(Name = "Prénom")]
            [StringLength(50, ErrorMessage = "Le champ Prénom est requis.", MinimumLength = 1)]
            public string FirstName { get; set; }
            [MaxLength(7)]
            [Display(Name = "Matricule")]
            public string Matricule { get; set; }
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Nom de l'entreprise")]
            public string EnterpriseName { get; set; }
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Rue (numéro inclus)")]
            public string StreetAdress { get; set; }
            [StringLength(10)]
            [DataType(DataType.PostalCode)]
            [Display(Name = "Code postal")]
            public string ZipCode { get; set; }
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Ville")]
            public string City { get; set; }
            [StringLength(50)]
            [DataType(DataType.Text)]
            [Display(Name = "Pays")]
            public string Country { get; set; }
            [Display(Name = "Permis le plus élevé")]
            public DriverLicense DriverLicense { get; set; }
            [Display(Name = "Niveau d'étude")]
            public StudyLevel StudyLevel { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Le mot de passe {0} doit avoir au minimum {2} caractères et avoir une taille max de {1}.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne sont pas les même.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var isValid = CheckFormFields();
            if (isValid != null)
            {
                return isValid;
            }

            if (ModelState.IsValid)
            {
                User user = getUserByRole(Input.role);

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user); // Pas utile mais je laisse au cas où
                    var currentUser = await _userManager.FindByEmailAsync(user.Email);

                    await assignUserToIdentityRoleAsync(Input.role, currentUser);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");

                    /*var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);*/
                    /*code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }*/
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private PageResult CheckFormFields()
        {
            bool error = false;
            switch(Input.role)
            {
                case "Client":
                    if(String.IsNullOrEmpty(Input.EnterpriseName)) 
                    {
                        ViewData["Error"] = " | Nom d'entreprise manquant";
                        error = true;
                    }
                    if(String.IsNullOrEmpty(Input.StreetAdress))
                    {
                        ViewData["Error"] += " | Adresse d'entreprise manquante";
                        error = true;
                    }
                    if(String.IsNullOrEmpty(Input.ZipCode))
                    {
                        ViewData["Error"] += " | Code postal manquant";
                        error = true;
                    }
                    if(String.IsNullOrEmpty(Input.City))
                    {
                        ViewData["Error"] += " | Ville manquante";
                        error = true;
                    }
                    if(String.IsNullOrEmpty(Input.Country))
                    {
                        ViewData["Error"] += " | Pays non sélectionné";
                        error = true;
                    }

                    if (error) return Page();
                    break;
                case "Dispatcher":
                    if (String.IsNullOrEmpty(Input.Matricule))
                    {
                        ViewData["Error"] = "Le Matricule ne peut être vide !";
                        error = true;
                    }
                    else if (!Regex.IsMatch(Input.Matricule, "^[A-Z]{1}[0-9]{6}"))
                    {
                        ViewData["Error"] = "Le Matricule ne respecte pas la structure \"X123456\" !";
                        error = true;
                    }
                    if (error) return Page();
                    break;
                case "Driver":
                    if (String.IsNullOrEmpty(Input.Matricule))
                    {
                        ViewData["Error"] = "Le Matricule ne peut être vide !";
                        error = true;
                    }
                    else if (!Regex.IsMatch(Input.Matricule, "^[A-Z]{1}[0-9]{6}"))
                    {
                        ViewData["Error"] = "Le Matricule ne respecte pas la structure \"X123456\" !";
                        error = true;
                    }
                    if(Input.DriverLicense == DriverLicense.B || Input.DriverLicense == DriverLicense.None)
                    {
                        ViewData["Error"] = " | Il vous faut au minimum le permis C pour être conducteur";
                        error = true;
                    }
                    if (error) return Page();
                    break;
                default:
                    throw new ArgumentException("Il faut pas jouer avec les champs hidden :(");
            }
            return null;
        }

        private async Task assignUserToIdentityRoleAsync(string role, User user)
        {
            switch (role)
            {
                case "Client":
                    var roleresult = await _userManager.AddToRoleAsync(user, "Client");
                    break;
                case "Driver":
                    roleresult = await _userManager.AddToRoleAsync(user, "Driver");
                    break;
                case "Dispatcher":
                    roleresult = await _userManager.AddToRoleAsync(user, "Dispatcher");
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private User getUserByRole(string role)
        {
            switch (role)
            {
                case "Client":
                    var client = Activator.CreateInstance<Client>();
                    client.FirstName = Input.FirstName;
                    client.LastName = Input.LastName;
                    client.EnterpriseAddress = $"{Input.StreetAdress}, {Input.ZipCode}, {Input.City}, {Input.Country}";
                    client.EnterpriseName = Input.EnterpriseName;
                    client.TwoFactorEnabled = false;
                    return client;
                case "Driver":
                    var driver = Activator.CreateInstance<Driver>();
                    driver.FirstName = Input.FirstName;
                    driver.LastName = Input.LastName;
                    driver.DriverLicense = Input.DriverLicense;
                    driver.TwoFactorEnabled = false;
                    return driver;
                case "Dispatcher":
                    var dispatcher = Activator.CreateInstance<Dispatcher>();
                    dispatcher.FirstName = Input.FirstName;
                    dispatcher.LastName = Input.LastName;
                    dispatcher.StudyLevel = Input.StudyLevel;
                    dispatcher.TwoFactorEnabled = false;
                    return dispatcher;
                default:
                    throw new ArgumentException();
            }
        }

        /*private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }*/

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
