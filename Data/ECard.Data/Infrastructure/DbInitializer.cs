using ECard.Entities.Entities;
using System.Linq;

namespace ECard.Data.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(ECardDataContext context)//SchoolContext is EF context
        {

            context.Database.EnsureCreated();//if db is not exist ,it will create database .but ,do nothing .
            SeedUser(context);
            SeedCardDetail(context);

        }

        private static void SeedUser(ECardDataContext context)
        {
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            User user = new User()
            {
                FirstName = "Admin",
                LastName = "Admin",
                Username = "Admin",
            };

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash("Admin123", out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            context.Users.Add(user);
            context.SaveChanges();
        }

        private static void SeedCardDetail(ECardDataContext context)
        {
            if (context.ECardDetail.Any())
            {
                return;   // DB has been seeded
            }

            ECardDetail eCardDetail = new ECardDetail()
            {
                Id_User = 1,
                Title = "Sulhi & Anisah",
                Description = "Sulhi & Anisah Wedding",
                DomainName = "sulhianisahwedding.com",
            };

            context.ECardDetail.Add(eCardDetail);
            context.SaveChanges();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
