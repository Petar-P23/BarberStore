using BarberStore.Infrastructure.Data.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberStore.Infrastructure.Data.Models;

public class Appointment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public DateTime Start { get; set; }
    [DefaultValue(false)]
    public Status Status { get; set; }
    public ApplicationUser User { get; set; }
    [Required]
    [ForeignKey(nameof(User))]
    public string? UserId { get; set; }
}