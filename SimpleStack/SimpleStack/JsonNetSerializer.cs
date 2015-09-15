using System;
using Newtonsoft.Json;

namespace SimpleStack
{
	public class JsonNetSerializer : IContentSerializer
	{
		public string Serialize(object item)
		{
			return JsonConvert.SerializeObject(item);
		}

		public T Deserialize<T>(string json) where T : class
		{
			return JsonConvert.DeserializeObject<T>(json) as T;
		}

		public object Deserialize(Type type, string json)
		{
			return JsonConvert.DeserializeObject(json, type);
		}
	}
}