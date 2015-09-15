using System;

namespace SimpleStack
{
	public interface IContentSerializer
	{
		string Serialize(object item);

		T Deserialize<T>(string value)
			where T : class;

		object Deserialize(Type type, string value);
	}
}