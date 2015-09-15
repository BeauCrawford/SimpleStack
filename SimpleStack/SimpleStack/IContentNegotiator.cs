using System.Web;

namespace SimpleStack
{
	public interface IContentNegotiator
	{
		string Negotiate(HttpContextBase http);
	}
}