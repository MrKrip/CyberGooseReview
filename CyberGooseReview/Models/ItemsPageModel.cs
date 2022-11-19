namespace CyberGooseReview.Models
{
    public class ItemsPageModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public PageModel PageModel { get; set; }

        public ItemsPageModel(IEnumerable<T> items, PageModel pageModel)
        {
            Items = items;
            PageModel = pageModel;
        }
    }
}
