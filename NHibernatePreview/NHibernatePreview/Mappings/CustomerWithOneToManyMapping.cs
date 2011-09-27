using FluentNHibernate.Mapping;
using NHibernatePreview.DomainModel;

namespace NHibernatePreview.Mappings
{
	public class CustomerWithOneToManyMapping : ClassMap<CustomerWithOneToMany>
	{
		public CustomerWithOneToManyMapping()
		{
			Table("CustomerWithOneToMany");
			Id(x => x.Id).GeneratedBy.GuidComb(); // http://davybrion.com/blog/2009/05/using-the-guidcomb-identifier-strategy/
			Map(x => x.Name).Not.Nullable();

			HasMany(x => x.Orders)
				.Table("OrdersWithManyToOne")
				.KeyColumn("CustomerId")
				.Cascade.AllDeleteOrphan()		// http://ayende.com/blog/1890/nhibernate-cascades-the-different-between-all-all-delete-orphans-and-save-update
				.Inverse()						// http://nhprof.com/Learn/Alerts/SuperfluousManyToOneUpdate
				.AsSet();						// http://ayende.com/blog/3943/nhibernate-mapping-set
		}
	}

	public class OrderWithManyToOneMapping : ClassMap<OrderWithManyToOne>
	{
		public OrderWithManyToOneMapping()
		{
			Table("OrdersWithManyToOne");
			Id(x => x.Id).GeneratedBy.GuidComb();
			Map(x => x.Name).Not.Nullable();

			References(x => x.Customer).Column("CustomerId");
		}
	}
}
