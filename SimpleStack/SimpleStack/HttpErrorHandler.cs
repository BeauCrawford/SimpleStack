using System.Net;
using System.Web;

namespace SimpleStack
{
	public class HttpErrorHandler : HttpBaseHandler
	{
		public HttpErrorHandler(HttpStatusCode code, string errorMessage)
		{
			Code = code;
			ErrorMessage = errorMessage;
		}

		public HttpStatusCode Code { get; private set; }
		public string ErrorMessage { get; private set; }

		public override void Process(HttpContextBase context)
		{
			context.Response.StatusCode = (int)Code;
			context.Response.Write(ErrorMessage);
		}
	}
}