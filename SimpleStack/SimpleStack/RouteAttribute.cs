using System;

namespace SimpleStack
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class RouteAttribute : Attribute
	{
		public RouteAttribute(string route)
		{
			Route = route;
		}

		public string Route { get; private set; }
	}
}