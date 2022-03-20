namespace BarberStore.Core.Models.Store;

public class CartProductViewModel : ProductViewModel
{
    public int Quantity { get; set; }
    public string ImagePath { get; set; }
}