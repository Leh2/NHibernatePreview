using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernatePreview.DomainModel;

namespace NHibernatePreview.Mappings.Overrides
{
	public class CustomerWithGuidOverrrides : IAutoMappingOverride<CustomerWithGuid>
	{
		public void Override(AutoMapping<CustomerWithGuid> mapping)
		{
			mapping.Id(x => x.Id).GeneratedBy.GuidComb();
		}
	}
}
