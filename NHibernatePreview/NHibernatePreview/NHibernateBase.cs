using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernatePreview.DomainModel;
using PropertyName = NHibernate.Cfg.Environment;

namespace NHibernatePreview
{
	public abstract class NHibernateBase
	{
		protected static Configuration _configuration;
		protected static ISessionFactory _sessionFactory;

		public NHibernateBase()
		{
			if (_sessionFactory == null)
			{
				var autoMappingConfiguration = AutoMap.AssemblyOf<NHibernateTests>()
					.Where(type => type.Namespace.Equals("NHibernatePreview.DomainModel"))
					.Conventions.AddFromAssemblyOf<NHibernateTests>()
					.UseOverridesFromAssemblyOf<NHibernateTests>()
					.IncludeBase<CustomerBaseWithTablePerHierarchyInheritance>();

				_configuration = Fluently.Configure()
					.Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromConnectionStringWithKey("NHibernatePreview"))
						.Raw(PropertyName.ProxyFactoryFactoryClass, typeof(ProxyFactoryFactory).AssemblyQualifiedName)
						.Raw(PropertyName.Isolation, "ReadUncommitted")
						.Raw(PropertyName.CurrentSessionContextClass, "thread_static")
						.Raw(PropertyName.GenerateStatistics, "true")
						.Raw(PropertyName.BatchSize, "50"))
					.Mappings(m =>
					{
						m.HbmMappings.AddFromAssemblyOf<NHibernateTests>();
						m.FluentMappings.AddFromAssemblyOf<NHibernateTests>();
						m.AutoMappings.Add(autoMappingConfiguration);
					})
					.BuildConfiguration();

				_sessionFactory = _configuration.BuildSessionFactory();

				NHibernateProfiler.Initialize();
			}
		}
	}
}