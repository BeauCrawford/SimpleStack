namespace SimpleStack.Web.Services.Products
{
	[Route("/Product/{ProductId}")]
	public class ProductRequest
	{
		public int ProductId { get; set; }
	}
}