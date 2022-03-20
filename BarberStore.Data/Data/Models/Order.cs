using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberStore.Infrastructure.Data.Models;

public class Order
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();
    public ApplicationUser User { get; set; }
    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public IEnumerable<Product> Products { get; set; } = new List<Product>();
    [Required]
    public DateTime TimeOfOrdering { get; set; }
}