using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernatePreview.DomainModel;

namespace NHibernatePreview.Mappings.Overrides
{
	public class CustomerWithVersionOverrrides : IAutoMappingOverride<CustomerWithVersion>
	{
		public void Override(AutoMapping<CustomerWithVersion> mapping)
		{
			mapping.Version(x => x.Version);
		}
	}
}