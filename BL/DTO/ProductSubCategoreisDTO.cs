namespace BLL.DTO
{
    public class ProductSubCategoreisDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductDTO Product { get; set; }
        public int SubCategoryID { get; set; }
        public SubCategoryDTO SubCategory { get; set; }
    }
}
