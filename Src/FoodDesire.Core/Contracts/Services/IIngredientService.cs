﻿namespace FoodDesire.Core.Contracts.Services;
public interface IIngredientService {
    Task<IngredientCategory> NewIngredientCategory(IngredientCategory ingredientCategory);
    Task<List<IngredientCategory>> GetAllIngredientCategories();
    Task<IngredientCategory> GetIngredientCategoryById(int ingredientCategoryId);
    Task<IngredientCategory> GetIngredientCategoryByName(string ingredientCategoryName);
    Task<bool> DeleteIngredientCategoryById(int ingredientCategoryId);

    Task<Ingredient> NewIngredient(Ingredient ingredient);
    Task<List<Ingredient>> GetAllIngredientsWithCategory();
    Task<Ingredient> GetIngredientById(int ingredientId);
    Task<Ingredient> GetIngredientByName(string ingredientName);
    Task<Ingredient> EditIngredient(Ingredient ingredient);
    Task<List<Ingredient>> GetAllIngredientsByCategory(string ingredientCategory);
    Task<bool> DeleteIngredientById(int ingredientCategoryId);

    Task<Supply> NewSupply(Supply supply);
    Task<IngredientCategory> EditIngredientCategory(IngredientCategory ingredientCategory);
    Task<List<Ingredient>> SearchIngredients(string searchText);
    Task<List<Ingredient>> GetAllIngredients();
}
