namespace Models.Dtos
{
    public class ProductDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
