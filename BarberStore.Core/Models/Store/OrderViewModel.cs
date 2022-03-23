
using BarberStore.Infrastructure.Data.Enums;

namespace BarberStore.Core.Models.Store;

public class OrderViewModel
{
    public string? Id { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public DateTime OrderTime { get; set; }
    public Status Status { get; set; }
    public IList<OrderProductViewModel> Products { get; set; } = new List<OrderProductViewModel>();
}