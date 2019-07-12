using ECard.Data.Infrastructure;
using ECard.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
    }

    public class RsvpService : IRsvpService
    {
        private ECardDataContext _context;

        public RsvpService(ECardDataContext context)
        {
            _context = context;
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
