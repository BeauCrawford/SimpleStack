using System.Linq;
using System.Web;
using NSubstitute;
using NUnit.Framework;

namespace SimpleStack.Tests
{
	class HttpHandlerFactoryTests
	{
		public void Go()
		{
			var http = Substitute.For<HttpContextBase>();
			http.Request.HttpMethod.Returns("GET");
			http.Request.Path.Returns("/Product/12345");

			var factory = new HttpHandlerFactory(typeof(SimpleService).Assembly);

			System.Console.WriteLine(factory.Routes.Count());

			var handler = factory.MapHandler(http);

			Assert.IsTrue(handler is HttpServiceHandler);

			var serviceHandler = handler as HttpServiceHandler;

			Assert.AreEqual("/Product/{ProductId}", serviceHandler.Route.Route);
		}

		public class SimpleService
		{
			public object Get(SimpleRequest request)
			{
				return 12345;
			}
		}

		[Route("/Product/{ProductId}")]
		public class SimpleRequest
		{
			public int ProductId { get; set; }
		}
	}
}
