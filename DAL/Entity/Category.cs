using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
