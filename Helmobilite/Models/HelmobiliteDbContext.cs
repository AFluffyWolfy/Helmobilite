using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Helmobilite.Models;
using Bogus;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Helmobilite.Models
{
    public class HelmobiliteDbContext : IdentityDbContext<User>
    {
        public HelmobiliteDbContext(DbContextOptions optionsBuilder) : base(optionsBuilder) { }

        public DbSet<Truck> Trucks { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Helmobilite.Models.Admin> Admins { get; set; }
        public DbSet<Helmobilite.Models.Client> Clients { get; set; }
        public DbSet<Helmobilite.Models.Dispatcher> Dispatcher { get; set; }
        public DbSet<Helmobilite.Models.Driver> Drivers { get; set; }
        public DbSet<Helmobilite.Models.TruckBrand> TruckBrand { get; set; }
        public DbSet<Helmobilite.Models.TruckModel> TruckModel { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Conversion des enums en String dans la table
            modelBuilder.Entity<Truck>().Property(e => e.LicenseType).HasConversion(v => v.ToString(), v => (DriverLicense)Enum.Parse(typeof(DriverLicense), v)).HasMaxLength(50);
            modelBuilder.Entity<Driver>().Property(e => e.DriverLicense).HasConversion(v => v.ToString(), v => (DriverLicense)Enum.Parse(typeof(DriverLicense), v)).HasMaxLength(50);
            modelBuilder.Entity<Dispatcher>().Property(e => e.StudyLevel).HasConversion(v => v.ToString(), v => (StudyLevel)Enum.Parse(typeof(StudyLevel), v)).HasMaxLength(50);

            //Ajouter la table User en TPH
            //modelBuilder.Entity<User>(u =>
            //{
            //    u.HasDiscriminator<string>("Role")
            //    .HasValue<Admin>("Admin")
            //    .HasValue<Client>("Client")
            //    .HasValue<Dispatcher>("Dispatcher")
            //    .HasValue<Driver>("Driver");

            //    u.Property("Role").HasMaxLength(50);
            //});

            // /!\ DECOMMENTER POUR SEED LA BDD
            Seed(modelBuilder);
        }

        public void Seed(ModelBuilder modelBuilder)
        {
            // Création des marques de camions
            List<TruckBrand> truckBrands = new List<TruckBrand>() { 
                new TruckBrand {Id = 1, Name = "Mercedes-Benz" },
                new TruckBrand {Id = 2, Name = "MAN"},
                new TruckBrand {Id = 3, Name = "IVECO"},
                new TruckBrand {Id = 4, Name = "DAF"},
                new TruckBrand {Id = 5, Name = "Volvo"} };
            for (int i = 0; i < 5; i++)
            {
                modelBuilder.Entity<TruckBrand>().HasData(truckBrands[i]);
            }

            // Création des modèles de camions
            List<TruckModel> truckModels = new List<TruckModel>() { 
                new TruckModel {Id = 1, Name = "M31" },
                new TruckModel {Id = 2, Name = "Atleon" },
                new TruckModel {Id = 3, Name = "Eurocargo" },
                new TruckModel {Id = 4, Name = "XF Euro 6" },
                new TruckModel {Id = 5, Name = "Peterbilt 389" } };
            for (int i = 0; i < 5; i++)
            {
                modelBuilder.Entity<TruckModel>().HasData(truckModels[i]);
            }

            string letterAndNumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; 
            // Créer 10 camions
            for (int i = 0; i < 10; i++)
            {
                int licensePlateFront = Randomizer.Seed.Next(1, 4);
                string licensePlateMiddle = "";
                string licensePlateEnd = "";
                for (int j = 0; j < 3; j++)
                {
                    licensePlateMiddle += letterAndNumbers[Randomizer.Seed.Next(0, letterAndNumbers.Length)];
                    licensePlateEnd += letterAndNumbers[Randomizer.Seed.Next(0, letterAndNumbers.Length)];
                }
                string licensePlate = licensePlateFront + "-" + licensePlateMiddle + "-" + licensePlateEnd;
                int brandId = Randomizer.Seed.Next(1, truckBrands.Count + 1);
                int modelId = Randomizer.Seed.Next(1, truckModels.Count + 1);
                int licenseType = Randomizer.Seed.Next(1, 3);
                int weight = (Randomizer.Seed.Next(15000, 30000) + 50) / 100 * 100;

                modelBuilder.Entity<Truck>().HasData(
                    new Truck
                    {
                        LicensePlate = licensePlate,
                        TruckBrandId = brandId,
                        TruckModelId = modelId,
                        LicenseType = (DriverLicense)licenseType,
                        MaxWeight = weight,
                        PicturePath = ""
                    });
            }
        }
    }
}