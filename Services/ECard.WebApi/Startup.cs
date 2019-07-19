using AutoMapper;
using ECard.Data.Infrastructure;
using ECard.WebApi.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Project.AspNetCore.JwtSecurity.Services.Implementations;
using Project.Framework.Constants;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Services;

namespace WebApi
{
	public class Startup
	{
		static AppSettings _appSettings = null;
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors();
			//services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDb"));
			services.AddDbContext<ECardDataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

			// configure strongly typed settings objects
			var appSettingsSection = Configuration.GetSection("AppSettings");
			services.Configure<AppSettings>(appSettingsSection);
			_appSettings = appSettingsSection.Get<AppSettings>();

			services
			  .AddJwtBearerAuthentication(options =>
			  {
				  options.Issuer = "yourIssuerCode";
				  options.IssuerSigningKey = _appSettings.Secret;
			  });

			services.AddJwtServer().AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddAutoMapper();


			// configure jwt authentication
			//_appSettings = appSettingsSection.Get<AppSettings>();
			//var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
			//services.AddAuthentication(x =>
			//{
			//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			//})
			//.AddJwtBearer(x =>
			//{
			//    x.Events = new JwtBearerEvents
			//    {
			//        OnTokenValidated = context =>
			//        {
			//            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
			//            var userId = int.Parse(context.Principal.Identity.Name);
			//            var user = userService.GetById(userId);
			//            if (user == null)
			//            {
			//                // return unauthorized if user no longer exists
			//                context.Fail("Unauthorized");
			//            }
			//            return Task.CompletedTask;
			//        }
			//    };
			//    x.RequireHttpsMetadata = false;
			//    x.SaveToken = true;
			//    x.TokenValidationParameters = new TokenValidationParameters
			//    {
			//        ValidateIssuerSigningKey = true,
			//        IssuerSigningKey = new SymmetricSecurityKey(key),
			//        ValidateIssuer = false,
			//        ValidateAudience = false
			//    };
			//});

			// configure DI for application services
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IECardDetailService, ECardDetailService>();
			services.AddScoped<IRsvpService, RsvpService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ECardDataContext ctx)
		{
			// global cors policy
			app.UseCors(x => x
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader());

			DbInitializer.Initialize(ctx);

			app.UseJwtServer(options =>
			{

				options.TokenEndpointPath = "/api/Token";
				options.AccessTokenExpireTimeSpan = new TimeSpan(1, 0, 0);
				options.Issuer = "yourIssuerCode";
				options.IssuerSigningKey = _appSettings.Secret;
				options.AuthorizationServerProvider = new AuthorizationServerProvider
				{
					OnGrantResourceOwnerCredentialsAsync = async (context) =>
					{
						var userService = context.Context.RequestServices.GetRequiredService<IUserService>();
						var user = userService.Authenticate(context.UserName, context.Password);
						if (user == null)
						{
							context.SetError("The user name or password is incorrect.");
							return;
						}

						var claims = new List<Claim>
						{
							new Claim(Constant.Claim.User.Id, user.Id.ToString()),
							new Claim(Constant.Claim.User.UserName, user.Username),
							new Claim(Constant.Claim.User.Name, $"{user.FirstName} {user.LastName}"),
						};

						context.Validated(claims);
						await Task.FromResult(0);
					}
				};
			});

			app.UseAuthentication();
			app.UseMvc();
		}
	}
}
