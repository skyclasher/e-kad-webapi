using ECard.Entities.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project.Framework.Interfaces;
using System;
using System.Linq;

namespace ECard.Data.Infrastructure
{
    public class ECardDataContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ECardDataContext(DbContextOptions<ECardDataContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public int DbContextSaveChanges()
        {
            return base.SaveChanges();
        }

        //Override DbContext SaveChanges() method to auto populate CreatedOn & ModifiedOn values.
        public override int SaveChanges()
        {
            var objectStateEntries = ChangeTracker.Entries()
                .Where(e => e.Entity is IEntity &&
                (e.State == EntityState.Modified || e.State == EntityState.Added))
                .ToList();

            var currentTime = DateTime.UtcNow;

            foreach (var entry in objectStateEntries)
            {
                var entityBase = entry.Entity as IEntity;
                if (entityBase == null)
                {
                    continue;
                }

                var context = _httpContextAccessor.HttpContext;
                if (entry.State == EntityState.Added)
                {
                    entityBase.CreatedBy = context == null ? "" : "";
                    entityBase.CreatedByName = context == null ? "" : "";
                    entityBase.ModifiedBy = context == null ? "" : "";
                    entityBase.ModifiedByName = context == null ? "" : "";
                    entityBase.CreatedDate = entityBase.CreatedDate == DateTime.MinValue ? currentTime : entityBase.CreatedDate;
                    entityBase.ModifiedDate = currentTime;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(entityBase.CreatedBy)).IsModified = false;
                    entry.Property(nameof(entityBase.CreatedByName)).IsModified = false;
                    entry.Property(nameof(entityBase.ModifiedBy)).IsModified = false;
                    entry.Property(nameof(entityBase.ModifiedByName)).IsModified = false;
                    entry.Property(nameof(entityBase.CreatedDate)).IsModified = false;
                    entityBase.ModifiedDate = currentTime;
                    entityBase.ModifiedBy = context == null ? "" : "";
                    entityBase.ModifiedByName = context == null ? "" : "";
                }
            }

            return DbContextSaveChanges();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Rsvp> Rsvp { get; set; }
        public DbSet<ECardDetail> ECardDetail { get; set; }
    }
}
