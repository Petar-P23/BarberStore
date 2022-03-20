using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class Article
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(ArticleTitleMaxLength)]
    public string? Title { get; set; }
    [Required]
    public string? MainText { get; set; }
    [MaxLength(ImagePathMaxLength)]
    public string? ImagePath { get; set; }
    [Required]
    public DateTime PublishDate { get; set; }
    public ApplicationUser PublishUser { get; set; }
    [ForeignKey(nameof(PublishUser))]
    public string? PublishUserId { get; set; }
}