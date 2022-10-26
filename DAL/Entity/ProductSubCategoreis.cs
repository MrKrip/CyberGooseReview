using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class ProductSubCategoreis
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SubCategoryID { get; set; }
        public SubCategory SubCategory { get; set; }
    }
}
