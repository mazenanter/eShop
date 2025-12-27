using System.ComponentModel.DataAnnotations;

namespace eShop.Dtos
{
    public class AddReviewDto
    {
        public int ProductId { get; set; }

        [Range(1, 5)]
        public double Rating { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }
    }
}
