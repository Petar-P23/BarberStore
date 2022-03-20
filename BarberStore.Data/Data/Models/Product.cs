﻿using System.ComponentModel.DataAnnotations;
using static BarberStore.Infrastructure.Data.Constants.ValidationConstants;

namespace BarberStore.Infrastructure.Data.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(ProductNameMaxLength)]
    public string? Name { get; set; }

    [MaxLength(ProductDescriptionMaxLength)]
    public string? Description { get; set; }
    [Required]
    public decimal Price { get; set; }

    public IEnumerable<Order> Orders { get; set; } = new List<Order>();
}