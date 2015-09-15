using NUnit.Framework;

namespace SimpleStack.Tests
{
	[TestFixture]
	public class RouteInfoTests
	{
		[Test]
		public void SinglePropertyParse()
		{
			var result = RouteInfo.Parse("/Product/{ProductId}", typeof(ProductRequest));

			Assert.AreEqual("^/Product/([0-9]+)$", result.Pattern.ToString());

			Assert.AreEqual(1, result.RouteGroups.Count);

			var property = result.RouteGroups[1];

			Assert.AreEqual("ProductId", property.Name);
		}

		[Test]
		public void PropertiesThatDoNotExist()
		{
			var result = RouteInfo.Parse("/Product/{PropA}/{PropB}", typeof(ProductRequest));

			Assert.AreEqual("^/Product/[a-zA-Z0-9]+/[a-zA-Z0-9]+$", result.Pattern.ToString());

			Assert.AreEqual(0, result.RouteGroups.Count);
		}

		public class ProductRequest
		{
			public int ProductId { get; set; }
		}
	}
}
