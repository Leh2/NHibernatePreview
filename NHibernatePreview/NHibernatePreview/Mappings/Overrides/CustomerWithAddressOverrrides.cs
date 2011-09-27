using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernatePreview.DomainModel;

namespace NHibernatePreview.Mappings.Overrides
{
	public class CustomerWithAddressOverrrides : IAutoMappingOverride<CustomerWithAddress>
	{
		public void Override(AutoMapping<CustomerWithAddress> mapping)
		{
			mapping.Component(x => x.Address, m =>
			{
				m.Map(x => x.Street);
				m.Map(x => x.ZipCode);
				m.Map(x => x.City);
				m.Map(x => x.State);
				m.Map(x => x.Country);
			});
		}
	}
}
