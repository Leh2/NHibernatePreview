using System;
using Iesi.Collections.Generic;

namespace NHibernatePreview.DomainModel
{
	public class CustomerWithOneToMany
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual ISet<OrderWithManyToOne> Orders { get; set; }

		public CustomerWithOneToMany()
		{
			Orders = new HashedSet<OrderWithManyToOne>();
		}

		public virtual void AddOrder(OrderWithManyToOne order)
		{
			Orders.Add(order);
			order.Customer = this;
		}
	}

	public class OrderWithManyToOne
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual CustomerWithOneToMany Customer { get; set; }

		public OrderWithManyToOne()
		{
			Customer = new CustomerWithOneToMany();
		}
	}
}
