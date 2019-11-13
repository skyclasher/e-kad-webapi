using ECard.Data.Infrastructure;
using ECard.Entities.DomainModels.Chart;
using ECard.Entities.Entities;
using ECard.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Project.Framework.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebApi.Helpers;

namespace WebApi.Services
{
	public interface IRsvpService
	{
		IEnumerable<Rsvp> GetAll();
		Rsvp GetById(int id);
		Rsvp Create(Rsvp rsvp);
		void Update(Rsvp rsvp);
		void UpdateByEmail(Rsvp rsvp);
		void Delete(int id);
		Rsvp GetByEmail(string email);
		Rsvp GetByEmailAndECardId(string email, int ecardId);
		List<Rsvp> GetByUserId(int userId);
		ChartData GetRsvpChartData(int userId);
		PagingHelper<Rsvp> GetPagedRsvpByUserId(int userId, int currentPage, string searchText);
		PagingHelper<Rsvp> GetPagedAttendRsvpByUserId(int userId, string searchText, int currentPage);
		PagingHelper<Rsvp> GetPagedNotAttendRsvpByUserId(int userId, string searchText, int currentPage);
	}

	public class RsvpService : IRsvpService
	{
		private ECardDataContext _context;
		private readonly AppSettings _appSettings;

		public RsvpService(ECardDataContext context, IOptions<AppSettings> appSettings)
		{
			_context = context;
			_appSettings = appSettings.Value;
		}

		public IEnumerable<Rsvp> GetAll()
		{
			return _context.Rsvp;
		}

		public Rsvp GetById(int id)
		{
			return _context.Rsvp.Find(id);
		}

		public Rsvp GetByEmail(string email)
		{
			return _context.Rsvp.Where(x => x.Email == email).SingleOrDefault();
		}

		public Rsvp GetByEmailAndECardId(string email, int ecardId)
		{
			return _context.Rsvp.Where(x => x.Email == email && x.Id_EcardDetail == ecardId).SingleOrDefault();
		}

		public List<Rsvp> GetByUserId(int userId)
		{
			ECardDetail ecard = _context.ECardDetail.Where(x => x.Id_User == userId).FirstOrDefault();

			if (ecard == null)
				return new List<Rsvp>() { };
			else
				return _context.Rsvp.Where(x => x.Id_EcardDetail == ecard.Id).ToList();
		}

		private PagingHelper<Rsvp> PagingRsvpByUserId(List<Rsvp> data, string searchText, int currentPage)
		{
			return PagingHelper<Rsvp>.ToPaging(data, x => x.OrderBy(y => y.Name), searchText, _appSettings.PageSize, currentPage);
		}

		public PagingHelper<Rsvp> GetPagedRsvpByUserId(int userId, int currentPage, string searchText)
		{
			int ecardId = _context.ECardDetail.Where(x => x.Id_User == userId).SingleOrDefault().Id;

			Expression<Func<Rsvp, bool>> filter =
				   x => (x.Id_EcardDetail == ecardId)
						&&
						(string.IsNullOrEmpty(searchText) ? true :
						   (x.Name.ToLower().Contains(searchText.ToLower()) ||
						   x.AttCount.ToString().Contains(searchText))
						);


			var data = _context.Rsvp.Where(filter).ToList();

			return PagingRsvpByUserId(data, searchText, currentPage);
		}


		public PagingHelper<Rsvp> GetPagedAttendRsvpByUserId(int userId, string searchText, int currentPage)
		{
			List<Rsvp> rsvps = new List<Rsvp>();
			ECardDetail ecard = _context.ECardDetail.Where(x => x.Id_User == userId).SingleOrDefault();

			if (ecard != null)
			{
				Expression<Func<Rsvp, bool>> filter =
					   x => (x.Id_EcardDetail == ecard.Id && x.Attendance == "H")
							&&
							(string.IsNullOrEmpty(searchText) ? true :
							   (x.Name.ToLower().Contains(searchText.ToLower()) ||
							   x.AttCount.ToString().Contains(searchText))
							);


				rsvps = _context.Rsvp.Where(filter).ToList();
			}

			return PagingRsvpByUserId(rsvps, searchText, currentPage);
		}


		public PagingHelper<Rsvp> GetPagedNotAttendRsvpByUserId(int userId, string searchText, int currentPage)
		{
			List<Rsvp> rsvps = new List<Rsvp>();
			ECardDetail ecard = _context.ECardDetail.Where(x => x.Id_User == userId).SingleOrDefault();

			if (ecard != null)
			{
				Expression<Func<Rsvp, bool>> filter =
					   x => (x.Id_EcardDetail == ecard.Id && x.Attendance != "H")
							&&
							(string.IsNullOrEmpty(searchText) ? true :
							   (x.Name.ToLower().Contains(searchText.ToLower()) ||
							   x.AttCount.ToString().Contains(searchText))
							);


				rsvps = _context.Rsvp.Where(filter).ToList();
			}

			return PagingRsvpByUserId(rsvps, searchText, currentPage);
		}


		public ChartData GetRsvpChartData(int userId)
		{
			List<Rsvp> rsvpList = GetByUserId(userId);
			ChartData chart = new ChartData();

			if (rsvpList.Count > 0)
			{
				var data = rsvpList.GroupBy(r => r.Attendance)
									   .Select(grp => new
									   {
										   value = grp.Count(),
										   color = grp.Key == "H" ? "#00a65a" : grp.Key == "M" ? "#f39c12" : grp.Key == "T" ? "#f56954" : "#d3d3d3",
										   highlight = grp.Key == "H" ? "#00a65a" : grp.Key == "M" ? "#f39c12" : grp.Key == "T" ? "#f56954" : "#d3d3d3",
										   label = grp.Key == "H" ? "Hadir" : grp.Key == "M" ? "Mungkin" : grp.Key == "T" ? "Tidak Hadir" : "Tiada Jawapan"
									   })
									   .OrderBy(o => o.label)
									   .ToList();



				//chart.element = Constant.Admin.AvgSessionChart.element;
				chart.resize = true;
				chart.jsonData = JsonConvert.SerializeObject(data);

			}

			return chart;
		}

		public Rsvp Create(Rsvp rsvp)
		{
			// validation
			if (string.IsNullOrWhiteSpace(rsvp.Email))
				throw new AppException("Email is required");

			Rsvp existing = GetByEmailAndECardId(rsvp.Email, rsvp.Id_EcardDetail);

			if (existing == null)
			{
				// check if local is not null 
				if (existing != null) // I'm using a extension method
				{
					// detach
					_context.Entry(existing).State = EntityState.Detached;
				}

				_context.Rsvp.Add(rsvp);
				_context.SaveChanges();
			}
			else
			{
				Update(rsvp);
			}

			return rsvp;
		}

		public void Update(Rsvp rsvp)
		{
			Rsvp existing = GetByEmailAndECardId(rsvp.Email, rsvp.Id_EcardDetail);

			if (existing == null)
				throw new AppException("Rsvp record not found");

			rsvp.Id = existing.Id;
			rsvp.Id_EcardDetail = existing.Id_EcardDetail;
			rsvp.ECardDetail = null;

			// check if local is not null 
			if (existing != null) // I'm using a extension method
			{
				// detach
				_context.Entry(existing).State = EntityState.Detached;
			}

			_context.Rsvp.Update(rsvp);
			_context.SaveChanges();
		}

		public void UpdateByEmail(Rsvp rsvp)
		{
			Rsvp existing = GetByEmail(rsvp.Email);

			if (rsvp == null)
				throw new AppException("Rsvp record not found");

			rsvp.Id = existing.Id;
			rsvp.Id_EcardDetail = existing.Id_EcardDetail;

			// check if local is not null 
			if (existing != null) // I'm using a extension method
			{
				// detach
				_context.Entry(existing).State = EntityState.Detached;
			}

			_context.Rsvp.Update(rsvp);
			_context.SaveChanges();
		}

		public void Delete(int id)
		{
			Rsvp rsvp1 = _context.Rsvp.Find(id);
			if (rsvp1 != null)
			{
				_context.Rsvp.Remove(rsvp1);
				_context.SaveChanges();
			}
		}


	}
}
