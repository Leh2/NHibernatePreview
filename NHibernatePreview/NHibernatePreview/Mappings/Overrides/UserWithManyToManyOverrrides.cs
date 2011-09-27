using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernatePreview.DomainModel;

namespace NHibernatePreview.Mappings.Overrides
{
	public class UserWithManyToManyOverrrides : IAutoMappingOverride<UserWithManyToMany>
	{
		public void Override(AutoMapping<UserWithManyToMany> mapping)
		{
			mapping.Id(x => x.Id).GeneratedBy.GuidComb();

			mapping.HasManyToMany(x => x.Roles)
				.Table("UserRoleRelation")
				.ParentKeyColumn("UserId")
				.ChildKeyColumn("RoleId")
				.Cascade.All()
				.Inverse()
				.AsSet();
		}
	}

	public class RoleWithManyToManyOverrrides : IAutoMappingOverride<RoleWithManyToMany>
	{
		public void Override(AutoMapping<RoleWithManyToMany> mapping)
		{
			mapping.Id(x => x.Id).GeneratedBy.GuidComb();

			mapping.HasManyToMany(x => x.Users)
				.Table("UserRoleRelation")
				.ParentKeyColumn("RoleId")
				.ChildKeyColumn("UserId")
				.Cascade.All()
				.AsSet();
		}
	}
}
