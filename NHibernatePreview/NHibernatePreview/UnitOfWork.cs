using NHibernate;
using NHibernate.Context;

namespace NHibernatePreview
{
	public class UnitOfWork : NHibernateBase, IUnitOfWork
	{
		private bool _commitWasCalled;

		public UnitOfWork()
		{
			BeginTransaction();
		}

		public ISession Session
		{
			get
			{
				return GetCurrentSession();
			}
		}

		public void Commit()
		{
			CommitTransaction();
			_commitWasCalled = true;
		}

		public void Dispose()
		{
			if (!_commitWasCalled)
			{
				RollbackTransaction();
			}

			DisposeSession();
		}

		private static ISession GetCurrentSession()
		{
			if (!CurrentSessionContext.HasBind(_sessionFactory))
			{
				CurrentSessionContext.Bind(_sessionFactory.OpenSession());
			}

			return _sessionFactory.GetCurrentSession();
		}

		private static void BeginTransaction()
		{
			GetCurrentSession().BeginTransaction();
		}

		private static void CommitTransaction()
		{
			var session = GetCurrentSession();
			if (session.Transaction.IsActive)
			{
				session.Transaction.Commit();
			}
		}

		private static void RollbackTransaction()
		{
			var session = GetCurrentSession();
			if (session != null && session.Transaction != null && session.Transaction.IsActive)
			{
				session.Transaction.Rollback();
			}
		}

		private static void DisposeSession()
		{
			var session = GetCurrentSession();
			if (session != null)
			{
				CurrentSessionContext.Unbind(_sessionFactory);

				if (session.IsOpen)
				{
					session.Close();
				}
				session.Dispose();
			}
		}
	}
}