using ECard.Data.Infrastructure;
using ECard.Entities.Entities;
using System.Collections.Generic;
using System.Linq;
using WebApi.Dtos;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IECardDetailService
    {
        IEnumerable<ECardDetail> GetAll();
        ECardDetail GetById(int id);
        ECardDetail Create(ECardDetail ecardDetail);
        void Update(ECardDetail user);
        void Delete(int id);
        ECardDetail GetECardDetailIdByTitleAndDomainName(string title, string domainName);
    }

    public class ECardDetailService : IECardDetailService
    {
        private ECardDataContext _context;

        public ECardDetailService(ECardDataContext context)
        {
            _context = context;
        }

        public IEnumerable<ECardDetail> GetAll()
        {
            return _context.ECardDetail;
        }

        public ECardDetail GetById(int id)
        {
            return _context.ECardDetail.Find(id);
        }

        public ECardDetail GetECardDetailIdByTitleAndDomainName(string title, string domainName)
        {
            return _context.ECardDetail.Where(x => x.Title == title && x.DomainName == domainName).FirstOrDefault();
        }

        public ECardDetail Create(ECardDetail eCardDetail)
        {
            // validation
            if (string.IsNullOrWhiteSpace(eCardDetail.Title))
                throw new AppException("Title is required");

            _context.ECardDetail.Add(eCardDetail);
            _context.SaveChanges();

            return eCardDetail;
        }

        public void Update(ECardDetail eCardDetail)
        {
            ECardDetail eCardDetail1 = _context.ECardDetail.Find(eCardDetail.Id);

            if (eCardDetail1 == null)
                throw new AppException("ECardDetail record not found");

            _context.ECardDetail.Update(eCardDetail1);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            ECardDetail eCardDetail1 = _context.ECardDetail.Find(id);
            if (eCardDetail1 != null)
            {
                _context.ECardDetail.Remove(eCardDetail1);
                _context.SaveChanges();
            }
        }
    }
}
