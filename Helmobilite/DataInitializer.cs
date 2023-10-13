using Bogus;
using Helmobilite.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Dispatcher = Helmobilite.Models.Dispatcher;

namespace Helmobilite
{
    public static class DataInitializer
    {
        public static void SeedRole(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.RoleExistsAsync("Client").Result == false) 
            {
                IdentityRole client = new IdentityRole() { Name = "Client" };
                var result = roleManager.CreateAsync(client);
                result.Wait();
            }
            if (roleManager.RoleExistsAsync("Driver").Result == false)
            {
                IdentityRole driver = new IdentityRole() { Name = "Driver" };
                var result = roleManager.CreateAsync(driver);
                result.Wait();
            }
            if (roleManager.RoleExistsAsync("Dispatcher").Result == false)
            {
                IdentityRole dispatcher = new IdentityRole() { Name = "Dispatcher" };
                var result = roleManager.CreateAsync(dispatcher);
                result.Wait();
            }
            if (roleManager.RoleExistsAsync("Admin").Result == false)
            {
                IdentityRole admin = new IdentityRole() { Name = "Admin" };
                var result = roleManager.CreateAsync(admin);
                result.Wait();
            }
        }

        public static async Task Seed(UserManager<User> _userManager)
        {
            if (_userManager.Users.Count() != 0)
                return;
            // Hardcode de 4 users qui ne changent jamais
            var finalDispatcher = new Dispatcher
            {
                FirstName = "Dispatcher",
                LastName = "Michel",
                Email = "Dispatcher.Michel@helmobilite.be",
                UserName = "Dispatcher.Michel@helmobilite.be",
                Matricule = "D" + "000001",
                StudyLevel = StudyLevel.Bachelier
            };
            var resultDispatcher = _userManager.CreateAsync(finalDispatcher, "Pass123$").Result;
            if (resultDispatcher.Succeeded)
            {
                var resultDispatcher2 = _userManager.AddToRoleAsync(finalDispatcher, "Dispatcher").Result;
            }

            var finalDriver = new Driver
            {
                FirstName = "Driver",
                LastName = "Michel",
                Email = "Driver.Michel@helmobilite.be",
                UserName = "Driver.Michel@helmobilite.be",
                Matricule = "C" + "000002",
                DriverLicense = DriverLicense.CE
            };
            var resultDriver = _userManager.CreateAsync(finalDriver, "Pass123$").Result;
            if (resultDriver.Succeeded)
            {
                var resultDriver2 = _userManager.AddToRoleAsync(finalDriver, "Driver").Result;
            }

            var finalClient = new Client
            {
                FirstName = "Client",
                LastName = "Michel",
                Email = "Client.Michel@helmobilite.be",
                UserName = "Client.Michel@helmobilite.be",
                EnterpriseAddress = "Rue des supermichels, n°42 Liège, 4000",
                EnterpriseName = "Les Michels Sympatoche"
            };
            var resultClient = _userManager.CreateAsync(finalClient, "Pass123$").Result;
            if (resultClient.Succeeded)
            {
                var resultClient2 = _userManager.AddToRoleAsync(finalClient, "Client").Result;
            }

            var finalAdmin = new Admin
            {
                FirstName = "Admin",
                LastName = "Michel",
                Email = "Admin.Michel@helmobilite.be",
                UserName = "Admin.Michel@helmobilite.be"
            };
            var resultAdmin = _userManager.CreateAsync(finalAdmin, "Pass123$").Result;
            if (resultAdmin.Succeeded)
            {
                var resultAdmin2 = _userManager.AddToRoleAsync(finalAdmin, "Admin").Result;
            }


            // Créer 2 dispatchers
            for (int i = 0; i < 2; i++)
            {
                var surName = new Bogus.Person().FirstName;
                var lastName = new Bogus.Person().LastName;
                var email = $"{surName}.{lastName}@helmobilite.be";
                var matricule = Randomizer.Seed.Next(100000, 999999);
                var studylevel = Randomizer.Seed.Next(0, 3);
                var dispatcher =
                    new Dispatcher
                    {
                        FirstName = surName,
                        LastName = lastName,
                        Email = email,
                        UserName = email,
                        Matricule = "D" + matricule,
                        StudyLevel = (StudyLevel)studylevel
                    };
                var result = _userManager.CreateAsync(dispatcher, "Pass123$").Result;
                if (result.Succeeded)
                {
                    var result2 = _userManager.AddToRoleAsync(dispatcher, "Dispatcher").Result;
                }
            }

            // Créer 5 chauffeurs
            for (int i = 0; i < 5; i++)
            {
                var surName = new Bogus.Person().FirstName;
                var lastName = new Bogus.Person().LastName;
                var email = $"{surName}.{lastName}@helmobilite.be";
                var matricule = Randomizer.Seed.Next(100000, 999999);
                var driverlicense = Randomizer.Seed.Next(1, 3);
                var driver =
                    new Driver
                    {
                        FirstName = surName,
                        LastName = lastName,
                        Email = email,
                        UserName = email,
                        Matricule = "C" + matricule,
                        DriverLicense = (DriverLicense)driverlicense
                    };
                var result = _userManager.CreateAsync(driver, "Pass123$").Result;
                if (result.Succeeded)
                {
                    var result2 = _userManager.AddToRoleAsync(driver, "Driver").Result;
                }
            }

            // Créer 5 clients
            for (int i = 0; i < 5; i++)
            {
                var surName = new Bogus.Person().FirstName;
                var lastName = new Bogus.Person().LastName;
                var email = $"{surName}.{lastName}@mymail.be";
                var driverlicense = Randomizer.Seed.Next(1, 3);
                var client = 
                    new Client
                    {
                        FirstName = surName,
                        LastName = lastName,
                        Email = email,
                        UserName = email,
                        EnterpriseName = "SomeEnterprise",
                        EnterpriseAddress = "Rue des nulls, Néant, n°0"
                    };
                var result = _userManager.CreateAsync(client, "Pass123$").Result;
                if (result.Succeeded)
                {
                    var result2 = _userManager.AddToRoleAsync(client, "Client").Result;
                }
            }

            //TODO: Créer 10 livraisons factices soit à la main
            // soit ici
        }


    }
}
