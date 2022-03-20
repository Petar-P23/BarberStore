using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class StaffMember
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(ImagePathMaxLength)]
    public string? ImagePath { get; set; }
    [Required]
    [MaxLength(StaffMemberPhoneNumberMaxLength)]
    public string? PhoneNumber { get; set; }
    public ApplicationUser User { get; set; }
    [Required]
    [ForeignKey(nameof(User))]
    public string? UserId { get; set; }
}