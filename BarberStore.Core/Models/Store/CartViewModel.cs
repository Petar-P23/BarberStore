namespace BarberStore.Core.Models.Store;

public class CartViewModel
{
    public string Id { get; set; }
    public IList<CartProductViewModel> Products { get; set; }
}