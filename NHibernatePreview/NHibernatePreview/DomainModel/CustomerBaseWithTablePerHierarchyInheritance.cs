
namespace NHibernatePreview.DomainModel
{
	public abstract class CustomerBaseWithTablePerHierarchyInheritance
	{
		public virtual int Id { get; set; }
		public virtual string CommonProperty { get; set; }
	}

	public class CustomerPersonWithTablePerHierarchyInheritance : CustomerBaseWithTablePerHierarchyInheritance
	{
		public virtual string FullName { get; set; }
	}

	public class CustomerCompanyWithTablePerHierarchyInheritance : CustomerBaseWithTablePerHierarchyInheritance
	{
		public virtual string CompanyName { get; set; }
	}
}