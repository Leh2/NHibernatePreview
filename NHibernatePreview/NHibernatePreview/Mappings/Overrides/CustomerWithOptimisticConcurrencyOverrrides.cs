using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernatePreview.DomainModel;

namespace NHibernatePreview.Mappings.Overrides
{
	public class CustomerWithOptimisticConcurrencyOverrrides : IAutoMappingOverride<CustomerWithOptimisticConcurrency>
	{
		public void Override(AutoMapping<CustomerWithOptimisticConcurrency> mapping)
		{
			// http://ayende.com/blog/3946/nhibernate-mapping-concurrency

			mapping.OptimisticLock.Dirty();
			mapping.DynamicUpdate();
		}
	}
}
