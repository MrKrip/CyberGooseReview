namespace CyberGooseReview.Models
{
    public class FilterPageModel<T, F>
    {
        public IEnumerable<T> Items { get; set; }
        public F Filter { get; set; }
        public PageModel PageModel { get; set; }

        public FilterPageModel(IEnumerable<T> items, F filter, PageModel pageModel)
        {
            Items = items;
            PageModel = pageModel;
            Filter = filter;
        }
        public FilterPageModel() { }
    }
}
