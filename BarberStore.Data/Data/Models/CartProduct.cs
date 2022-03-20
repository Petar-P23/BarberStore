using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberStore.Infrastructure.Data.Models;

public class CartProduct
{
    public Cart Cart { get; set; }
    [ForeignKey(nameof(Cart))]
    public Guid CartId { get; set; }
    public Product Product { get; set; }
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    [Required]
    [DefaultValue(1)]
    public int Quantity { get; set; }
}