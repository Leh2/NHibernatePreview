using System;
using NHibernate;

namespace NHibernatePreview
{
	public interface IUnitOfWork : IDisposable
	{
		ISession Session
		{
			get;
		}

		void Commit();
	}
}
