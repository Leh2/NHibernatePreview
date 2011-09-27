
namespace NHibernatePreview.DomainModel
{
	public abstract class CustomerBaseWithTablePerSubclassInheritance
	{
		public virtual int Id { get; set; }
		public virtual string CommonProperty { get; set; }
	}

	public class CustomerPersonWithTablePerSubclassInheritance : CustomerBaseWithTablePerSubclassInheritance
	{
		public virtual string FullName { get; set; }
	}

	public class CustomerCompanyWithTablePerSubclassInheritance : CustomerBaseWithTablePerSubclassInheritance
	{
		public virtual string CompanyName { get; set; }
	}
}
