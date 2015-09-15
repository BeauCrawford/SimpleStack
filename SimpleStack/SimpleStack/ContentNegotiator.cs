using System.Web;

namespace SimpleStack
{
	public class ContentNegotiator : IContentNegotiator
	{
		public string Negotiate(HttpContextBase http)
		{
			return "application/json";
		}
	}
}