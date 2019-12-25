using AutoMapper;
using ECard.Business.XloRecords;
using ECard.Data.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Project.AspNetCore.JwtSecurity.Services.Implementations;
using Project.Framework.Configuration;
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

		public IConfiguration Configuration;

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors();
			//services.AddDbContext<ECardDataContext>(x => x.UseMySql("Server=localhost;User Id=ekad;Password=wL#$gBW@8?fjDPmdm6ef;Database=ekad"));
			services.AddDbContext<ECardDataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

			// configure strongly typed settings objects
			var appSettingsSection = Configuration.GetSection("AppSettings");
			services.Configure<AppSettings>(appSettingsSection);
			_appSettings = appSettingsSection.Get<AppSettings>();

			services
			  .AddJwtBearerAuthentication(options =>
			  {
				  options.Issuer = "yourIssuerCode";
				  //options.IssuerSigningKey = "fVGGS9A&3ULP$P-U5aFRGge!RmBRhRCENMY+A3Ckq2E2%HwVqC#^x7w*aU4B3P&ZE52A!uzCUtn+&E48nnY46YPt*^Ne5VwU%LG&w9qmxG$+9LrYPzz5_kDkF$FW2NCe5ud+xKh7Uka%DbcGukp=-pgXr!=wZ@rWvQSc^L%rn@3Qp^CT8Jz=wNF$f8=vA2zY2X9XSJd*3@AkpgSz=^##DFhtCnqn&5D^xVgZj$y5-&BbBPuzrga^UUndQ*^&nCPj";
				  options.IssuerSigningKey = _appSettings.Secret;
			  });

			services.AddJwtServer().AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddAutoMapper();


			// configure DI for application services
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IECardDetailService, ECardDetailService>();
			services.AddScoped<IRsvpService, RsvpService>();
			//services.AddScoped<IGenericComponent, GenericComponent>();
			services.AddScoped<IXloRecordComponent, XloRecordComponent>();
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
				//options.IssuerSigningKey = "fVGGS9A&3ULP$P-U5aFRGge!RmBRhRCENMY+A3Ckq2E2%HwVqC#^x7w*aU4B3P&ZE52A!uzCUtn+&E48nnY46YPt*^Ne5VwU%LG&w9qmxG$+9LrYPzz5_kDkF$FW2NCe5ud+xKh7Uka%DbcGukp=-pgXr!=wZ@rWvQSc^L%rn@3Qp^CT8Jz=wNF$f8=vA2zY2X9XSJd*3@AkpgSz=^##DFhtCnqn&5D^xVgZj$y5-&BbBPuzrga^UUndQ*^&nCPj";
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
