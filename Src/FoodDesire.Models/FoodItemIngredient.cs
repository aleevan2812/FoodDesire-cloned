﻿namespace FoodDesire.Models;
public sealed class FoodItemIngredient {
    [AllowNull]
    public int? RecipeId { get; set; }
    [AllowNull]
    public int? IngredientId { get; set; }
    [Required, NotNull]
    public double Amount { get; set; }
    [Required, NotNull]
    public double RecommendedMultiplier { get; set; }
    [Required, NotNull]
    public bool IsRequired { get; set; } = true;
    [Required, NotNull]
    [Column(TypeName = "Decimal(18,2)")]
    public decimal PricePerMultiplier { get; set; }
    public double Multiplier { get; set; } = 1;
    [Required]
    public bool IsRecipe => (RecipeId != null);
}