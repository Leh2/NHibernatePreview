using FluentNHibernate.Mapping;
using NHibernatePreview.DomainModel;

namespace NHibernatePreview.Mappings.Overrides
{
	public class CustomerBaseWithTablePerHierarchyInheritanceMapping : ClassMap<CustomerBaseWithTablePerHierarchyInheritance>
	{
		public CustomerBaseWithTablePerHierarchyInheritanceMapping()
		{
			Table("CustomerBaseWithTablePerHierarchyInheritance");
			Id(x => x.Id).GeneratedBy.Identity();
			Map(x => x.CommonProperty);
			DiscriminateSubClassesOnColumn("ClassType");
		}
	}

	public class CustomerPersonWithTablePerHierarchyInheritanceSubclassMapping : SubclassMap<CustomerPersonWithTablePerHierarchyInheritance>
	{
		public CustomerPersonWithTablePerHierarchyInheritanceSubclassMapping()
		{
			DiscriminatorValue("Person");
			Map(x => x.FullName);
		}
	}

	public class CustomerCompanyWithTablePerHierarchyInheritanceSubclassMapping : SubclassMap<CustomerCompanyWithTablePerHierarchyInheritance>
	{
		public CustomerCompanyWithTablePerHierarchyInheritanceSubclassMapping()
		{
			DiscriminatorValue("Company");
			Map(x => x.CompanyName);
		}
	}
}