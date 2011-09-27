using System;

namespace NHibernatePreview.DomainModel
{
	public class CustomerWithOptimisticConcurrency
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime BornAt { get; set; }
	}
}
