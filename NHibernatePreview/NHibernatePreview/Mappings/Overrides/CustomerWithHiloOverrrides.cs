using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernatePreview.DomainModel;

namespace NHibernatePreview.Mappings.Overrides
{
	public class CustomerWithHiloOverrrides : IAutoMappingOverride<CustomerWithHilo>
	{
		public void Override(AutoMapping<CustomerWithHilo> mapping)
		{
			//	http://www.philliphaydon.com/2010/10/using-hilo-with-fluentnhibernate/

			mapping.Id(x => x.Id).GeneratedBy
				.HiLo(table: "HiLo", column: "NextHigh", maxLo: "50");
		}
	}
}