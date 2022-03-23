using BarberStore.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public ApplicationUser User { get; set; }
    [ForeignKey(nameof(User))]
    public string? UserId { get; set; }
    [Required]
    public DateTime TimeOfOrdering { get; set; }
    [Required]
    [MaxLength(OrderAddressMaxLength)]
    public string? Address { get; set; }
    public Status Status { get; set; } = Status.Pending;
    public IList<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}