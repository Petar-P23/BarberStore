using System.ComponentModel.DataAnnotations;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class Announcement
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(AnnouncementMainTextMaxLength)]
    public string? MainText { get; set; }
    [Required]
    public DateTime PublishDate { get; set; }
}