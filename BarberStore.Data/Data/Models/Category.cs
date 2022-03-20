using System.ComponentModel.DataAnnotations;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(CategoryNameMaxLength)]
    public string? Name { get; set; }
    public IList<Product> Products { get; set; } = new List<Product>();
}