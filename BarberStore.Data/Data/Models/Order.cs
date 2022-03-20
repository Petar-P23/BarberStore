﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberStore.Infrastructure.Data.Models;

public class Order
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();
    public ApplicationUser User { get; set; }
    [ForeignKey(nameof(User))]
    public string? UserId { get; set; }
    public IEnumerable<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    [Required]
    public DateTime TimeOfOrdering { get; set; }
}