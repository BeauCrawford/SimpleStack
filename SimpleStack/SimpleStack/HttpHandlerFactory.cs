using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Autofac;

namespace SimpleStack
{
	public class HttpHandlerFactory : IHttpHandlerFactory
	{
		private static readonly IContainer container;
		private static readonly List<RouteInfo> routes = new List<RouteInfo>();

		static HttpHandlerFactory()
		{
			routes.AddRange(new RouteFinder().FindAll(AppDomain.CurrentDomain.GetAssemblies()));
			container = CreateContainer(routes);
		}

		private static IContainer CreateContainer(IEnumerable<RouteInfo> routes)
		{
			var builder = new ContainerBuilder();
			builder.RegisterType<JsonNetSerializer>().As<IJsonSerializer>();
			builder.RegisterType<ContentNegotiator>().As<IContentNegotiator>();
			builder.Register((c) => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>();
			builder.RegisterType<HttpServiceHandler>();
			builder.RegisterType(typeof(HttpErrorHandler));
			builder.Register((c) => container).As<IContainer>();

			foreach (var route in routes)
			{
				builder.RegisterType(route.Method.DeclaringType);
				builder.RegisterType(route.RequestType);
			}

			return builder.Build();
		}

		private T CreateInstance<T>(params object[] args)
			where T : class
		{
			var parameters = new List<TypedParameter>();

			foreach (var arg in args)
			{
				parameters.Add(new TypedParameter(arg.GetType(), arg));
			}

			return container.Resolve<T>(parameters.ToArray());
		}

		public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
		{
			var http = container.Resolve<HttpContextBase>();
			var matchingRoutes = routes.Where(r => r.IsMatch(http));
			var matchCount = matchingRoutes.Count();

			if (matchCount == 1)
			{
				return CreateInstance<HttpServiceHandler>(container, matchingRoutes.First());
			}
			else if (matchCount == 0)
			{
				return CreateInstance<HttpErrorHandler>(HttpStatusCode.NotFound);
			}
			else
			{
				return CreateInstance<HttpErrorHandler>(HttpStatusCode.InternalServerError);
			}
		}

		public void ReleaseHandler(IHttpHandler handler)
		{
		}
	}
}