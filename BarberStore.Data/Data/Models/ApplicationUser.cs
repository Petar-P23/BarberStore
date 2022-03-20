using System.ComponentModel.DataAnnotations;
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

    public IEnumerable<Order> Orders { get; set; }
    public IEnumerable<Appointment> Appointments { get; set; }

}