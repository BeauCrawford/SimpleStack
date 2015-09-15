using System;
using System.Web;

namespace SimpleStack.Web.Services.Products
{
	public class ProductService
	{
		public ProductService(HttpContextBase http, ServiceOptions options)
		{
			Http = http;
		}

		public HttpContextBase Http { get; private set; }

		public ProductResponse Get(ProductSearchRequest request)
		{
			var response = new ProductResponse();
			response.Message = DateTime.Now.ToFileTimeUtc().ToString();
			return response;
		}

		public ProductResponse Any(ProductRequest request)
		{
			var response = new ProductResponse();
			response.Message = "POST: " + DateTime.Now.ToFileTimeUtc();
			return response;
		}
	}
}