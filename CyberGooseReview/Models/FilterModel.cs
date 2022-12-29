namespace CyberGooseReview.Models
{
    public class FilterModel
    {
        public IEnumerable<CheckElementModel> SubCategory { get; set; }
        public IEnumerable<CheckElementModel> Country { get; set; }
        public int minRating { get; set; }
        public int maxRating { get; set; }
        public int minYear { get; set; }
        public int maxYear { get; set; }
    }
}
