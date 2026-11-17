namespace Products.Api.Dtos
{
    public class ProductDto
    {
      
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, RegularExpression(@"^[A-Za-z]{3}-\d{3}$")]
        public string Sku { get; set; } = "";       // unique in the database (POST-3)

        [Required]
        public string Category { get; set; } = "";  // GET-2

        [Range(0.01, 1000000)]
        public decimal Price { get; set; }          // PUT-4

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
