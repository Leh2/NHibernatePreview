using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using NHibernatePreview.DomainModel;
using Xunit;

namespace NHibernatePreview
{
	public class NHibernateTests : NHibernateBase, IDisposable
	{
		[Fact]
		public void Create_Database_Schema()
		{
			using (var session = _sessionFactory.OpenSession())
			{
				if (!session.Connection.ConnectionString.Contains("Mvno"))
				{
					var schemaExport = new SchemaExport(_configuration);
					schemaExport.Execute(script: false,
										 export: true,
										 justDrop: false,
										 connection: session.Connection,
										 exportOutput: Console.Out);
				}
			}
		}

		[Fact]
		public void Can_Not_Batch_Inserts_With_Identity()
		{
			// http://nhforge.org/blogs/nhibernate/archive/2009/03/20/nhibernate-poid-generators-revealed.aspx

			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction(IsolationLevel.ReadUncommitted))
			{
				for (int i = 0; i < 20; i++)
				{
					var customer = new CustomerWithHbmMapping { Name = string.Format("Customer: {0}", i) };
					session.Save(customer);
				}

				transaction.Commit();
			}

			Assert.Equal(20, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Can_Batch_Inserts_With_HiLo()
		{
			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction(IsolationLevel.ReadUncommitted))
			{
				for (int i = 0; i < 20; i++)
				{
					var customer = new CustomerWithHilo { Name = string.Format("Customer: {0}", i) };
					session.Save(customer);
				}

				transaction.Commit();
			}

			Assert.Equal(20, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Can_Batch_Inserts_With_Guid()
		{
			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction(IsolationLevel.ReadUncommitted))
			{
				for (int i = 0; i < 20; i++)
				{
					var customer = new CustomerWithGuid { Name = string.Format("Customer: {0}", i) };
					session.Save(customer);
				}

				transaction.Commit();
			}

			Assert.Equal(20, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Can_Use_Unit_Of_Work_Instead()
		{
			// http://nhforge.org/wikis/patternsandpractices/nhibernate-and-the-unit-of-work-pattern.aspx

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				for (int i = 0; i < 20; i++)
				{
					var customer = new CustomerWithGuid { Name = string.Format("Customer: {0}", i) };
					unitOfWork.Session.Save(customer);
				}

				unitOfWork.Commit();
			}

			Assert.Equal(20, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Can_Customer_Add_Orders_In_A_One_To_Many_bidirectional_Association()
		{
			// Forgetting to set the inverse true on CustomerWithOneToMany will cause unneccessary update statements

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				var customer = new CustomerWithOneToMany { Name = "Customer" };
				unitOfWork.Session.Save(customer);

				for (int i = 0; i < 10; i++)
				{
					var order = new OrderWithManyToOne { Name = string.Format("Order: {0}", i) };
					customer.AddOrder(order);
				}

				unitOfWork.Commit();
			}

			Assert.Equal(11, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Can_User_Add_And_Delete_Roles_In_A_Many_To_Many_bidirectional_Association()
		{
			UserWithManyToMany detachedUser = null;

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				detachedUser = new UserWithManyToMany { Name = "User" };
				unitOfWork.Session.Save(detachedUser);

				for (int i = 0; i < 5; i++)
				{
					var role = new RoleWithManyToMany { Name = string.Format("Order: {0}", i) };
					detachedUser.AddRole(role);
				}

				unitOfWork.Commit();
			}

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				var user = unitOfWork.Session.Query<UserWithManyToMany>().Fetch(x => x.Roles).Single(x => x.Id == detachedUser.Id);
				var role = user.Roles.ElementAt(0);
				user.RemoveRole(role);

				unitOfWork.Commit();
			}

			Assert.Equal(6, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Can_Map_Component_And_Get_Versus_Load()
		{
			var customer = new CustomerWithAddress
			{
				Name = "Customer",
				Address = { Street = "Gade 20", ZipCode = "2000", City = "Frederiksberg", State = string.Empty, Country = "Danmark", }
			};

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				unitOfWork.Session.Save(customer);	
				unitOfWork.Commit();
			}

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				//var loadedCustomer = unitOfWork.Session.Get<CustomerWithAddress>(customer.Id);
				//Assert.Equal(customer.Id, loadedCustomer.Id);

				var loadedCustomer = unitOfWork.Session.Load<CustomerWithAddress>(customer.Id);
				Assert.Equal(customer.Id, loadedCustomer.Id);

				unitOfWork.Commit();
			}

			Assert.Equal(1, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Can_Use_Versioned_Concurrency()
		{
			CustomerWithVersion detachedCustomer;

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				detachedCustomer = new CustomerWithVersion { Name = "Customer" };
				unitOfWork.Session.Save(detachedCustomer);
				unitOfWork.Commit();
			}

			Assert.Throws<StaleObjectStateException>(() => 
			{
				using (IUnitOfWork unitOfWork = new UnitOfWork())
				{
					var customer = unitOfWork.Session.Get<CustomerWithVersion>(detachedCustomer.Id);

					using (var statelessSession = _sessionFactory.OpenStatelessSession())
					using (var statelessTransaction = statelessSession.BeginTransaction())
					{
						var statelessCustomer = statelessSession.Get<CustomerWithVersion>(detachedCustomer.Id);
						statelessCustomer.Name = DateTime.Now.ToString();

						statelessSession.Update(statelessCustomer);
						statelessTransaction.Commit();
					}

					customer.Name = DateTime.Now.ToString();
					unitOfWork.Commit();
				}
			});
		}

		[Fact]
		public void Can_Use_Optimistic_Concurrency()
		{
			// http://ayende.com/blog/3946/nhibernate-mapping-concurrency

			CustomerWithOptimisticConcurrency detachedCustomer;

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				detachedCustomer = new CustomerWithOptimisticConcurrency { Name = "Customer", BornAt = 12.September(1985) };
				unitOfWork.Session.Save(detachedCustomer);
				unitOfWork.Commit();
			}

			Assert.Throws<StaleObjectStateException>(() =>
			{
				using (IUnitOfWork unitOfWork = new UnitOfWork())
				{
					var customer = unitOfWork.Session.Get<CustomerWithOptimisticConcurrency>(detachedCustomer.Id);
					customer.Name = DateTime.Now.ToString();

					using (var statelessSession = _sessionFactory.OpenStatelessSession())
					using (var statelessTransaction = statelessSession.BeginTransaction())
					{
						var statelessCustomer = statelessSession.Get<CustomerWithOptimisticConcurrency>(detachedCustomer.Id);
						statelessCustomer.Name = DateTime.Now.ToString();
						statelessSession.Update(statelessCustomer);
						statelessTransaction.Commit();
					}

					unitOfWork.Commit();
				}
			});
		}

		[Fact]
		public void Can_Use_Pessimistic_Concurrency()
		{
			CustomerWithGuid detachedCustomer;

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				detachedCustomer = new CustomerWithGuid { Name = "Customer" };
				unitOfWork.Session.Save(detachedCustomer);
				unitOfWork.Commit();
			}

			Assert.Throws<GenericADOException>(() =>
			{
				using (IUnitOfWork unitOfWork = new UnitOfWork())
				{
					var customer = unitOfWork.Session.Get<CustomerWithGuid>(detachedCustomer.Id, LockMode.Upgrade); // with (updlock, rowlock)

					using (var statelessSession = _sessionFactory.OpenStatelessSession())
					using (var statelessTransaction = statelessSession.BeginTransaction())
					{
						var statelessCustomer = statelessSession.Get<CustomerWithGuid>(detachedCustomer.Id);
						statelessCustomer.Name = DateTime.Now.ToString();
						statelessSession.Update(statelessCustomer);
						statelessTransaction.Commit();
					}

					customer.Name = DateTime.Now.ToString();
					unitOfWork.Commit();
				}
			});
		}

		[Fact]
		public void Can_Map_Table_Per_Hierarchy_Inheritance()
		{
			// http://ayende.com/blog/3941/nhibernate-mapping-inheritance
			// http://www.philliphaydon.com/2011/08/fluent-nhibernate-table-inheritance-discriminators
			// http://www.philliphaydon.com/2011/08/fluent-nhibernate-table-inheritance-discriminators-part-2

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				var company = new CustomerCompanyWithTablePerHierarchyInheritance { CompanyName = "Telenor", CommonProperty = "Random text" };
				unitOfWork.Session.Save(company);

				var person = new CustomerPersonWithTablePerHierarchyInheritance { FullName = "Customer", CommonProperty = "Another random text" };
				unitOfWork.Session.Save(person);

				unitOfWork.Commit();
			}

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				var companies = unitOfWork.Session.Query<CustomerCompanyWithTablePerHierarchyInheritance>().ToList();
				var people = unitOfWork.Session.Query<CustomerPersonWithTablePerHierarchyInheritance>().ToList();

				unitOfWork.Commit();
			}

			Assert.Equal(2, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Can_Map_Table_Per_Subclass_Inheritance()
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				var company = new CustomerCompanyWithTablePerSubclassInheritance { CompanyName = "Telenor", CommonProperty = "Company" };
				unitOfWork.Session.Save(company);

				var person = new CustomerPersonWithTablePerSubclassInheritance { FullName = "Customer", CommonProperty = "Person" };
				unitOfWork.Session.Save(person);

				unitOfWork.Commit();
			}

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				var companies = unitOfWork.Session.Query<CustomerCompanyWithTablePerSubclassInheritance>().ToList();
				var people = unitOfWork.Session.Query<CustomerPersonWithTablePerSubclassInheritance>().ToList();

				unitOfWork.Commit();
			}

			Assert.Equal(2, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Common_Select_N_Plus_One_Issue()
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				for (int i = 0; i < 10; i++)
				{
					var customer = new CustomerWithOneToMany { Name = string.Format("Customer: {0}", i) };
					unitOfWork.Session.Save(customer);

					var order = new OrderWithManyToOne { Name = "Order" };
					customer.AddOrder(order);
				}

				unitOfWork.Commit();
			}

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				var customers = unitOfWork.Session.QueryOver<CustomerWithOneToMany>().List();
				foreach (var customer in customers)
				{
					foreach (var order in customer.Orders)
					{
						var name = order.Name;
					}
				}

				unitOfWork.Commit();
			}

			Assert.Equal(20, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Avoiding_Select_N_Plus_One_Issue()
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				for (int i = 0; i < 10; i++)
				{
					var customer = new CustomerWithOneToMany { Name = string.Format("Customer: {0}", i) };
					unitOfWork.Session.Save(customer);

					var order = new OrderWithManyToOne { Name = "Order" };
					customer.AddOrder(order);
				}

				unitOfWork.Commit();
			}

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				// HQL (Hibernate Query Language)
				var customers = unitOfWork.Session.CreateQuery("from CustomerWithOneToMany customer left join fetch customer.Orders")
					.List<CustomerWithOneToMany>();

				// Criteria Api
				unitOfWork.Session.Evict(customers);
				customers = unitOfWork.Session.CreateCriteria<CustomerWithOneToMany>()
					.SetFetchMode("Orders", FetchMode.Eager)
					.List<CustomerWithOneToMany>();

				// QueryOver 
				unitOfWork.Session.Evict(customers);
				customers = unitOfWork.Session.QueryOver<CustomerWithOneToMany>()
					.Fetch(x => x.Orders).Eager
					.List();

				// Linq
				unitOfWork.Session.Evict(customers);
				customers = unitOfWork.Session.Query<CustomerWithOneToMany>()
					.Fetch(x => x.Orders)
					.ToList();

				foreach (var customer in customers)
				{
					foreach (var order in customer.Orders)
					{
						var name = order.Name;
					}
				}
			}

			Assert.Equal(20, _sessionFactory.Statistics.EntityInsertCount);
		}

		[Fact]
		public void Can_Use_Future_Queries()
		{
			// http://ayende.com/blog/3979/nhibernate-futures

			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				for (int i = 0; i < 10; i++)
				{
					var post = new CustomerWithGuid { Name = string.Format("Customer: {0}", i) };
					unitOfWork.Session.Save(post);
				}

				unitOfWork.Commit();
			}

			// Without futures
			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				var customers = unitOfWork.Session.QueryOver<CustomerWithGuid>()
					.Take(5)
					.List();

				var customerCount = unitOfWork.Session.QueryOver<CustomerWithGuid>()
					.RowCount();

				unitOfWork.Commit();
			}

			// With futures
			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				IEnumerable<CustomerWithGuid> customers = unitOfWork.Session.QueryOver<CustomerWithGuid>()
					.Take(5)
					.Future<CustomerWithGuid>();

				IFutureValue<int> customerCount = unitOfWork.Session.QueryOver<CustomerWithGuid>()
					.Select(Projections.RowCount())
					.FutureValue<int>();

				var count = customerCount.Value;

				unitOfWork.Commit();
			}

			Assert.Equal(10, _sessionFactory.Statistics.EntityInsertCount);
		}

		public void Dispose()
		{
			_sessionFactory.Statistics.Clear();
		}
	}
}