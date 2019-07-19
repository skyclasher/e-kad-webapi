using ECard.Data.Infrastructure;
using ECard.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using WebApi.Services;

namespace CreateUserConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
								.SetBasePath(Directory.GetCurrentDirectory())
								.AddJsonFile("appsettings.json");
			var configuration = builder.Build();

			var optionsBuilder = new DbContextOptionsBuilder<ECardDataContext>();
			optionsBuilder.UseMySql(configuration["conn"]);

			ECardDataContext eCardDataContext = new ECardDataContext(optionsBuilder.Options);
			IUserService userService = new UserService(eCardDataContext);

			Console.Write("Enter First Name: ");
			string firstName = Console.ReadLine();
			Console.Write("Enter Last Name: ");
			string lastName = Console.ReadLine();
			Console.Write("Enter username: ");
			string username = Console.ReadLine();
			Console.Write("Enter password: ");

			string pass = "";
			do
			{
				ConsoleKeyInfo key = Console.ReadKey(true);
				// Backspace Should Not Work
				if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
				{
					pass += key.KeyChar;
					Console.Write("*");
				}
				else
				{
					if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
					{
						pass = pass.Substring(0, (pass.Length - 1));
						Console.Write("\b \b");
					}
					else if (key.Key == ConsoleKey.Enter)
					{
						break;
					}
				}
			} while (true);

			User user = new User()
			{
				FirstName = firstName,
				LastName = lastName,
				Username = username,
			};

			userService.Create(user, pass);

			Console.WriteLine("User creation completed.");
		}
	}
}
