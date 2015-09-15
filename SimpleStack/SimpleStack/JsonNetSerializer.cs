using Newtonsoft.Json;

namespace SimpleStack
{
	public class JsonNetSerializer : IJsonSerializer
	{
		public string Serialize(object item)
		{
			return JsonConvert.SerializeObject(item);
		}
	}
}