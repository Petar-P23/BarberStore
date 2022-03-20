using System.ComponentModel.DataAnnotations;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class Service
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(ServiceNameMaxLength)]
    public string? Name { get; set; }
    [Required]
    public decimal Price { get; set; }
    [MaxLength(ServiceDescriptionMaxLength)]
    public string? Description { get; set; }
    public IList<Appointment> Appointments { get; set; } = new List<Appointment>();
}