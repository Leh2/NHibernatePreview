using System;
using Iesi.Collections.Generic;

namespace NHibernatePreview.DomainModel
{
	public class UserWithManyToMany
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<RoleWithManyToMany> Roles { get; set; }

		public UserWithManyToMany()
		{
			Roles = new HashedSet<RoleWithManyToMany>();
		}

		public virtual void AddRole(RoleWithManyToMany role)
		{
			Roles.Add(role);
			role.Users.Add(this);	
		}

		public virtual void RemoveRole(RoleWithManyToMany role)
		{
			Roles.Remove(role);
			role.Users.Remove(this);
		}
	}
}
