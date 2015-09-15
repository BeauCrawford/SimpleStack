using System.Net;
using System.Web;

namespace SimpleStack
{
	public class HttpErrorHandler : HttpBaseHandler
	{
		public HttpErrorHandler(HttpStatusCode code)
		{
			Code = code;
		}

		public HttpStatusCode Code { get; private set; }

		public override void Process(HttpContextBase context)
		{
			context.Response.StatusCode = (int)Code;
		}
	}
}