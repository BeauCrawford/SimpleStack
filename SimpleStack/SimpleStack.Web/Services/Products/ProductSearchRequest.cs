namespace SimpleStack.Web.Services.Products
{
	[Route("/Product/Search/Name/{Name}")]
	[Route("/Product/Search/Category/{Category}")]
	public class ProductSearchRequest
	{
		public string Category { get; set; }
		public string Name { get; set; }
	}
}