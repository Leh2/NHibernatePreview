using System;

namespace NHibernatePreview
{
	public static class DateTimeExtensions
	{
		public static DateTime September(this int day, int year)
		{
			return new DateTime(year, 9, day);
		}
	}
}
