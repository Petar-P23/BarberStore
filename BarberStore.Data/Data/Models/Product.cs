using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(ProductNameMaxLength)]
    public string? Name { get; set; }

    [MaxLength(ProductDescriptionMaxLength)]
    public string? Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    public Category Category { get; set; }
    [ForeignKey(nameof(Category))]
    public Guid CategoryId { get; set; }

    public IEnumerable<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    public IEnumerable<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
}