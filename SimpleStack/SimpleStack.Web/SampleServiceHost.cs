using Autofac;

namespace SimpleStack.Web
{
	public class SampleServiceHost : ServiceHost
	{
		protected override void BuildContainer(ContainerBuilder builder)
		{
		}

		protected override ServiceOptions GetOptions()
		{
			return new ServiceOptions();
		}

		protected override IContentNegotiator GetContentNegotiator()
		{
			return new ContentNegotiator();
		}
	}
}