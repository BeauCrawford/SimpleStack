using System;

namespace SimpleStack
{
	public class JsonContentType : ContentType
	{
		public override string Name
		{
			get
			{
				return "application/json";
			}
		}

		public override string GetDefaultBody(Type type)
		{
			return "{}";
		}

		public override IContentSerializer CreateSerializer()
		{
			return new JsonNetSerializer();
		}
	}
}