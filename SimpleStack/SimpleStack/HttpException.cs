using System;
using System.Net;

namespace SimpleStack
{
	public class HttpException : Exception
	{
		public HttpException(HttpStatusCode code)
		{
			StatusCode = code;
		}

		public HttpStatusCode StatusCode { get; private set; }
	}
}
