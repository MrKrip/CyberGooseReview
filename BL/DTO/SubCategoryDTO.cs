namespace BLL.DTO
{
    public class SubCategoryDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public CategoryDTO Category { get; set; }
        public string Name { get; set; }
    }
}
