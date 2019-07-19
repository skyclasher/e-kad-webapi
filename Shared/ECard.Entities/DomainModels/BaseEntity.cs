using Project.Framework.Interfaces;
using System;

namespace ECard.Entities.Entities
{
	public class BaseEntity<TId> : IEntity<TId>
	{
		public TId Id { get; set; }
		public DateTime CreatedDate { get; set; }

		public DateTime ModifiedDate { get; set; }

		public string CreatedBy { get; set; }

		public string CreatedByName { get; set; }

		public string ModifiedBy { get; set; }

		public string ModifiedByName { get; set; }
	}
}
