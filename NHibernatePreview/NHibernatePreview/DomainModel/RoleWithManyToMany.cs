using System;
using Iesi.Collections.Generic;

namespace NHibernatePreview.DomainModel
{
	public class RoleWithManyToMany
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<UserWithManyToMany> Users { get; set; }

		public RoleWithManyToMany()
		{
			Users = new HashedSet<UserWithManyToMany>();
		}

		public virtual void AddUser(UserWithManyToMany user)
		{
			Users.Add(user);
			user.Roles.Add(this);
		}

		public virtual void RemoveUser(UserWithManyToMany user)
		{
			Users.Remove(user);
			user.Roles.Remove(this);
		}
	}
}
