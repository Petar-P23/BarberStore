using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberStore.Infrastructure.Data.Models;

public class OrderProduct
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Order Order { get; set; }
    [ForeignKey(nameof(Order))]
    public Guid OrderId { get; set; }
    public Product Product { get; set; }
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    [Required]
    [DefaultValue(1)]
    public int Quantity { get; set; }
}