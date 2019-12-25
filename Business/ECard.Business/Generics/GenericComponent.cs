using ECard.Data.Infrastructure;
using Microsoft.Extensions.Options;
using Project.Framework.Configuration;
using Project.Framework.Exceptions;
using Project.Framework.Interfaces;
using System.Collections.Generic;

namespace ECard.Business.Generics
{
	public class GenericComponent<T> : IGenericComponent<T>
		where T : class, IEntity<int>
	{
		private ECardDataContext _context;
		private readonly AppSettings _appSettings;

		public GenericComponent(ECardDataContext context, IOptions<AppSettings> appSettings)
		{
			_context = context;
			_appSettings = appSettings.Value;
		}

		public T Create(T entity)
		{
			// validation
			if (entity == null)
				throw new AppException("entity cannot be null.");

			_context.Add(entity);
			_context.SaveChanges();

			return entity;
		}

		public void Delete(int id)
		{
			T entity = _context.Find<T>(id);
			if (entity == null)
				throw new RecordNotFoundException($"Entity record with Id {id} cannot find.");

			_context.Remove(entity);
			_context.SaveChanges();
		}

		public IEnumerable<T> GetAll()
		{
			return _context.Set<T>();
		}

		public T GetById(int id)
		{
			return _context.Find<T>(id);
		}

		public void Update(T entity)
		{
			var oriEntity = GetById(entity.Id);
			if (oriEntity == null)
				throw new RecordNotFoundException($"Entity record with Id {entity.Id} cannot find.");

			_context.Entry(oriEntity).CurrentValues.SetValues(entity);
			_context.Entry(oriEntity).Property(x => x.CreatedDate).IsModified = false;
			_context.SaveChanges();
		}
	}
}
