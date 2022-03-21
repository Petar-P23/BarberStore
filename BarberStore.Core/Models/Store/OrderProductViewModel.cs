namespace BarberStore.Core.Models.Store;

public class OrderProductViewModel : ProductViewModel
{
    public int Quantity { get; set; }
    public string? ImagePath { get; set; }
}