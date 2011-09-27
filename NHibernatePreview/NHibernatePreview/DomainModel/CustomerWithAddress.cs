using System;
using NHibernatePreview.DomainModel.Components;

namespace NHibernatePreview.DomainModel
{
	public class CustomerWithAddress
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual Address Address { get; set; }

		public CustomerWithAddress()
		{
			Address = new Address();
		}
	}
}