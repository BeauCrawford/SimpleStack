using System;

namespace SimpleStack
{
	public abstract class ContentType
	{
		protected ContentType()
		{
		}

		public abstract string Name { get; }
		public abstract string GetDefaultBody(Type type);
		public abstract IContentSerializer CreateSerializer();
	}
}