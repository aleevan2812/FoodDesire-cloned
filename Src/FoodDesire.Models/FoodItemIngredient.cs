﻿namespace FoodDesire.Models;
public class FoodItemIngredient {
    [AllowNull]
    public int? Recipe_Id { get; set; }
    public string? Recipe_Name { get; set; }
    [AllowNull]
    public int? Ingredient_Id { get; set; }
    public string? Ingredient_Name { get; set; }
    [Required]
    public double Amount { get; set; }
    [Required]
    public Measurement Measurement { get; set; }
    [Required]
    public double RecommendedMultiplier { get; set; }
    [Required]
    public bool IsRequired { get; set; } = true;
    [Required]
    public bool CanModify { get; set; } = false;
    [Required]
    [Column(TypeName = "Decimal(18,2)")]
    public decimal PricePerMultiplier { get; set; }
    public double Multiplier { get; set; } = 1;

    public double TotalAmount => Amount * Multiplier;
}
