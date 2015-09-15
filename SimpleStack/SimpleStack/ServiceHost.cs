using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace SimpleStack
{
	public class ServiceHost
	{
		public ServiceHost()
		{
		}

		protected internal virtual void BuildContainer(ContainerBuilder builder)
		{
		}

		protected internal virtual ServiceOptions GetOptions()
		{
			return new ServiceOptions();
		}

		protected internal virtual IContentNegotiator GetContentNegotiator()
		{
			return new ContentNegotiator();
		}

		public IEnumerable<RouteInfo> GetRoutes(params Assembly[] assemblies)
		{
			return new RouteLocator().FindAll(assemblies);
		}
	}
}