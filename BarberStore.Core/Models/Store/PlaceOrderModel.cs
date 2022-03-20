namespace BarberStore.Core.Models.Store;

public class PlaceOrderModel
{
    public string UserId { get; set; }
    public string Address { get; set; }
    public string[] ProductIds { get; set; }
}