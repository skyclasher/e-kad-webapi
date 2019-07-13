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
        List<Rsvp> GetByUserId(int userId);
        ChartData GetRsvpChartData(int userId);
        PagingHelper<Rsvp> GetPagedRsvpByUserId(int userId, int currentPage, string searchText);
        PagingHelper<Rsvp> GetPagedAttendRsvpByUserId(int userId, string searchText, int currentPage);
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

        public List<Rsvp> GetByUserId(int userId)
        {
            int ecardId = _context.ECardDetail.Where(x => x.Id_User == userId).SingleOrDefault().Id;

            return _context.Rsvp.Where(x => x.Id_EcardDetail == ecardId).ToList();
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
            int ecardId = _context.ECardDetail.Where(x => x.Id_User == userId).SingleOrDefault().Id;

            Expression<Func<Rsvp, bool>> filter =
                   x => (x.Id_EcardDetail == ecardId && x.Attendance == "H")
                        &&
                        (string.IsNullOrEmpty(searchText) ? true :
                           (x.Name.ToLower().Contains(searchText.ToLower()) ||
                           x.AttCount.ToString().Contains(searchText))
                        );


            var data = _context.Rsvp.Where(filter).ToList();

            return PagingRsvpByUserId(data, searchText, currentPage);
        }


        public ChartData GetRsvpChartData(int userId)
        {
           List<Rsvp> rsvpList = GetByUserId(userId);

            var data = rsvpList.GroupBy(r => r.Attendance)
                                   .Select(grp => new
                                   {
                                       value = grp.Count(),
                                       color = grp.Key == "H" ? "#00a65a" : grp.Key == "M" ? "#f39c12" : "#f56954",
                                       highlight = grp.Key == "H" ? "#00a65a" : grp.Key == "M" ? "#f39c12" : "#f56954",
                                       label = grp.Key == "H" ? "Hadir" : grp.Key == "M" ? "Mungkin" : "Tidak Hadir"
                                   })
                                   .OrderBy(o => o.label)
                                   .ToList();


            ChartData chart = new ChartData();

            //chart.element = Constant.Admin.AvgSessionChart.element;
            chart.resize = true;
            chart.jsonData = JsonConvert.SerializeObject(data);

            return chart;

        }

        public Rsvp Create(Rsvp rsvp)
        {
            // validation
            if (string.IsNullOrWhiteSpace(rsvp.Email))
                throw new AppException("Email is required");

            Rsvp existing = GetByEmail(rsvp.Email);

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
                UpdateByEmail(rsvp);
            }

            return rsvp;
        }

        public void Update(Rsvp rsvp)
        {
            Rsvp rsvp1 = _context.Rsvp.Find(rsvp.Id);

            if (rsvp1 == null)
                throw new AppException("Rsvp record not found");

            _context.Rsvp.Update(rsvp1);
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
