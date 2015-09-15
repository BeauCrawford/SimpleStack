using System;
using System.Net;
using System.Web;
using Autofac;

namespace SimpleStack
{
	public sealed class HttpServiceHandler : HttpBaseHandler
	{
		public HttpServiceHandler(IContainer container, RouteInfo route)
		{
			Guard.NotNull(container, "container");
			Guard.NotNull(route, "route");

			Container = container;
			Route = route;
		}

		public IContainer Container { get; private set; }

		public RouteInfo Route { get; private set; }

		public override void Process(HttpContextBase http)
		{
			try
			{
				var negotiator = Container.Resolve<IContentNegotiator>();
				var serializer = Container.Resolve<IJsonSerializer>();
				var service = Container.Resolve(Route.Method.DeclaringType);
				var request = Route.CreateRequest(Container, http);
				var response = Route.Method.Invoke(service, new object[] { request });

				http.Response.ContentType = negotiator.Negotiate(http);
				http.Response.Write(serializer.Serialize(response));
			}
			catch (HttpException httpError)
			{
				http.Response.StatusCode = (int)httpError.StatusCode;
				http.Response.Write(httpError.ToString());
			}
			catch (Exception ex)
			{
				http.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				http.Response.Write(ex.ToString());
			}
		}
	}
}