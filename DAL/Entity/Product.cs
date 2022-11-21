using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string Year { get; set; }
        public string Country { get; set; }
        public IEnumerable<ProductSubCategories> SubCategories { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
