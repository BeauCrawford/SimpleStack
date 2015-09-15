using System.Web;

namespace SimpleStack
{
	public abstract class HttpBaseHandler : IHttpHandler
	{
		public HttpBaseHandler()
		{
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			Process(new HttpContextWrapper(context));
		}

		public abstract void Process(HttpContextBase context);
	}
}