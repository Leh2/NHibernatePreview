using System;

namespace NHibernatePreview.DomainModel
{
	public class CustomerWithGuid
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
