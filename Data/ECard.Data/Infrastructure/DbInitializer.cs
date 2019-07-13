using Bogus;
using ECard.Entities.Entities;
using System.Collections.Generic;
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
            SeedRsvp(context);

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
        private static void SeedRsvp(ECardDataContext context)
        {
            if (context.Rsvp.Any())
            {
                return;   // DB has been seeded
            }

            var attendance = new List<string>
            {
                "H",
                "M",
                "T",
            };
            var rsvp = new Faker<Rsvp>()
                .RuleFor(o => o.Id_EcardDetail, 1)
                .RuleFor(o => o.Name, (f, u) => f.Name.FullName())
                .RuleFor(o => o.TelNo, (f, u) => f.Phone.PhoneNumber())
                .RuleFor(o => o.Wishes, f => f.Lorem.Sentence())
                .RuleFor(o => o.Email, f => f.Internet.Email())
                .RuleFor(o => o.Attendance, f => f.PickRandom(attendance))
                .RuleFor(o => o.AttCount, f => f.Random.Int(1, 10));


            context.Rsvp.AddRange(rsvp.Generate(200));
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
