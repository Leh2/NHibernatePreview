using FluentNHibernate.Mapping;
using NHibernatePreview.DomainModel;

namespace NHibernatePreview.Mappings.Overrides
{
	public class CustomerBaseWithTablePerSubclassInheritanceMapping : ClassMap<CustomerBaseWithTablePerSubclassInheritance>
	{
		public CustomerBaseWithTablePerSubclassInheritanceMapping()
		{
			Table("CustomerBaseWithTablePerSubclassInheritance");
			Id(x => x.Id).GeneratedBy.Identity();
			Map(x => x.CommonProperty);
		}
	}

	public class CustomerCompanyWithTablePerSubclassInheritanceSubclassMapping : SubclassMap<CustomerCompanyWithTablePerSubclassInheritance>
	{
		public CustomerCompanyWithTablePerSubclassInheritanceSubclassMapping()
		{
			KeyColumn("CompanyId");
			Map(x => x.CompanyName);
		}
	}

	public class CustomerPersonWithTablePerSubclassInheritanceSubclassMapping : SubclassMap<CustomerPersonWithTablePerSubclassInheritance>
	{
		public CustomerPersonWithTablePerSubclassInheritanceSubclassMapping()
		{
			KeyColumn("PersonId");
			Map(x => x.FullName);
		}
	}
}
