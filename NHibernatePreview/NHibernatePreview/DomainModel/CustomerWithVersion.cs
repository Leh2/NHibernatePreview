using System;

namespace NHibernatePreview.DomainModel
{
	public class CustomerWithVersion
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Version { get; set; }
	}
}