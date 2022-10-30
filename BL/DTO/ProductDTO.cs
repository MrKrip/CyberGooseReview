namespace BLL.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public CategoryDTO Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
