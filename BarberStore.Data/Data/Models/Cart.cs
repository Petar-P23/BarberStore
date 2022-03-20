using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberStore.Infrastructure.Data.Models;

public class Cart
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public ApplicationUser User { get; set; }
    [Required]
    [ForeignKey(nameof(User))]
    public string? UserId { get; set; }
    public IList<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
}