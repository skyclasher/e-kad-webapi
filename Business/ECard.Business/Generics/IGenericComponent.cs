using System.Collections.Generic;

namespace ECard.Business.Generics
{
	public interface IGenericComponent<T> where T : class
	{
		IEnumerable<T> GetAll();
		T GetById(int id);
		T Create(T rsvp);
		void Update(T rsvp);
		void Delete(int id);
	}
}
