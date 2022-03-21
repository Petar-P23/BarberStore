namespace BarberStore.Core.Models.Store;

public class StorePageViewModel
{
    public int PageNumber { get; set; }
    public int PagesCount { get; set; }
    public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
}