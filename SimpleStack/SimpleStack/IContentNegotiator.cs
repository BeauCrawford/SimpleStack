using System.Web;

namespace SimpleStack
{
	public interface IContentNegotiator
	{
		ContentType GetRequestContentType(HttpContextBase http);
		ContentType GetResponseContentType(HttpContextBase http);
	}
}