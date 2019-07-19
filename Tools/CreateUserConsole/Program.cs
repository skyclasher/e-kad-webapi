using ECard.Data.Infrastructure;
using ECard.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Services;

namespace CreateUserConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<ECardDataContext>();
			optionsBuilder.UseMySql("Server=localhost;User Id=root;Password=root;Database=ecard");

			ECardDataContext eCardDataContext = new ECardDataContext(optionsBuilder.Options);
			IUserService userService = new UserService(eCardDataContext);
			User user = new User()
			{
				FirstName = "Ahmad Sulhi",
				LastName = "Anuar",
				Username = "asulhi",
			};

			userService.Create(user, "Jr9AThxZ");
		}
	}
}
