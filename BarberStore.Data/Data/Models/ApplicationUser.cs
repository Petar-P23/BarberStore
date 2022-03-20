using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(UserNamesMaxLength)]
    public string? FirstName { get; set; }
    [Required]
    [MaxLength(UserNamesMaxLength)]
    public string? LastName { get; set; }

    public Cart Cart { get; set; }
    [ForeignKey(nameof(Cart))]
    public Guid CartId { get; set; }

    public IEnumerable<Order> Orders { get; set; }
    public IEnumerable<Appointment> Appointments { get; set; }

}