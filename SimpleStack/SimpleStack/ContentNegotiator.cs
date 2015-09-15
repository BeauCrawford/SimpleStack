using System.Web;

namespace SimpleStack
{
	public class ContentNegotiator : IContentNegotiator
	{
		public ContentType GetRequestContentType(HttpContextBase http)
		{
			return new JsonContentType();
		}

		public ContentType GetResponseContentType(HttpContextBase http)
		{
			return new JsonContentType();
		}
	}
}