using System.ComponentModel.DataAnnotations;

namespace eShop.Dtos
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile? MainImageUrl { get; set; }
        public List<IFormFile>? Images { get; set; }
        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }
        [Range(0, int.MaxValue)]
        public int Qty { get; set; }
        public int CategoryId { get; set; }
 
    }
}
