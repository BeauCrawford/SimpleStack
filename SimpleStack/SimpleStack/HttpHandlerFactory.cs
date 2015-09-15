using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using Autofac;

namespace SimpleStack
{
	public sealed class HttpHandlerFactory : IHttpHandlerFactory
	{
		public static class Messages
		{
			public const string AmbiguousRoutes = "Request Path is ambiguous";
			public const string NoRoute = "No Route exists for the following path: {0}";
		}

		private readonly List<RouteInfo> _routes = new List<RouteInfo>();
		private readonly ServiceOptions _serviceOptions;
		private readonly IContainer _container;
		private readonly ServiceHost _host;

		public IEnumerable<RouteInfo> Routes { get; private set; }

		public HttpHandlerFactory(params Assembly[] assemblies)
		{
			_host = GetServiceHost(assemblies);
			_routes.AddRange(_host.GetRoutes(assemblies));
			_serviceOptions = _host.GetOptions();
			_container = CreateContainer(_routes);

			Routes = new ReadOnlyCollection<RouteInfo>(_routes);
		}

		public HttpHandlerFactory()
			: this(AppDomain.CurrentDomain.GetAssemblies())
		{
		}

		private ServiceHost GetServiceHost(params Assembly[] assemblies)
		{
			var types = new List<Type>();

			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && typeof(ServiceHost).IsAssignableFrom(t)))
				{
					types.Add(type);
				}
			}

			var others = types.Where(t => typeof(ServiceHost) != t);

			var otherCount = others.Count();

			if (otherCount == 1)
			{
				return Activator.CreateInstance(others.First()) as ServiceHost;
			}
			else if (otherCount > 1)
			{
				throw new InvalidOperationException();
			}
			else
			{
				return new ServiceHost();
			}
		}

		private IContainer CreateContainer(IEnumerable<RouteInfo> routes)
		{
			var builder = new ContainerBuilder();
			builder.Register((c) => _serviceOptions).As<ServiceOptions>();
			builder.Register((c) => _host.GetContentNegotiator()).As<IContentNegotiator>();
			builder.Register((c) => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>();
			builder.RegisterType<HttpServiceHandler>();
			builder.RegisterType(typeof(HttpErrorHandler));
			builder.Register((c) => _container).As<IContainer>();

			foreach (var route in routes)
			{
				builder.RegisterType(route.Method.DeclaringType);
				builder.RegisterType(route.RequestType);
			}

			_host.BuildContainer(builder);

			return builder.Build();
		}

		public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
		{
			try
			{
				return MapHandler(new HttpContextWrapper(context));
			}
			catch (Exception ex)
			{
				return new HttpErrorHandler(HttpStatusCode.InternalServerError, ex.ToString());
			}
		}

		public IHttpHandler MapHandler(HttpContextBase http)
		{
			Guard.NotNull(http, "http");

			var matchingRoutes = _routes.Where(r => r.IsMatch(http));

			var matchCount = matchingRoutes.Count();

			if (matchCount == 1)
			{
				return _container.CreateInstance<HttpServiceHandler>(_container, matchingRoutes.First());
			}
			else if (matchCount == 0)
			{
				return _container.CreateInstance<HttpErrorHandler>(HttpStatusCode.NotFound, string.Format(Messages.NoRoute, http.Request.Path));
			}
			else
			{
				return _container.CreateInstance<HttpErrorHandler>(HttpStatusCode.InternalServerError, Messages.AmbiguousRoutes);
			}
		}

		public void ReleaseHandler(IHttpHandler handler)
		{
		}
	}
}