using System.ComponentModel.DataAnnotations;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class Store
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(StoreAddressMaxLength)]
    public string? Address { get; set; }
    [MaxLength(ImagePathMaxLength)]
    public string? ImagePath { get; set; }
    [Required]
    public DateTime OpeningTime { get; set; }
    [Required]
    public DateTime ClosingTime { get; set; }
}