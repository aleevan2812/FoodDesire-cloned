﻿namespace FoodDesire.Core.Test;
[TestFixture]
public class RecipeServices : Services {
    public RecipeServices() : base("RecipeServices") { }

    [OneTimeSetUp]
    public async Task SetUp() {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();

        await _chefService.CreateAccount(UserDataHelper.GetChefPayload());
        await _ingredientService.NewIngredientCategory(IngredientDataHelper.GetIngredientCategoryPayload());
        foreach (var ingredient in RecipeDataHelper.GetIngredients()) {
            await _ingredientService.NewIngredient(ingredient);
        }
    }

    [OneTimeTearDown]
    public async Task TearDown() {
        await _context.Database.EnsureDeletedAsync();
        ApplicationHostHelper.TearDownHost();
    }

    [Test, Order(1)]
    public async Task NewRecipeCategory() {
        RecipeCategory recipeCategory = await _recipeService.NewRecipeCategory(RecipeDataHelper.GetRecipeCategoryPayload());
        Assert.That(recipeCategory.Name, Is.EqualTo(RecipeDataHelper.GetRecipeCategoryPayload().Name));
    }

    [Test, Order(2)]
    public async Task NewRecipe() {
        Recipe recipe = RecipeDataHelper.GetRecipes()[0];

        Recipe newRecipe = await _recipeService.NewRecipe(recipe);
        Assert.Multiple(() => {
            Assert.That(newRecipe.Name, Is.EqualTo(recipe.Name));
            Assert.That(newRecipe.RecipeIngredients, Has.Count.EqualTo(recipe.RecipeIngredients.Count));
        });
    }

    [Test, Order(3)]
    public async Task GetRecipes() {
        Recipe recipe = await _recipeService.GetRecipeById(1);
        List<Recipe> recipes = await _recipeService.GetAllRecipesWithCategory();

        Assert.Multiple(() => {
            Assert.That(recipe.Name, Is.EqualTo(RecipeDataHelper.GetRecipes()[0].Name));
            Assert.That(recipe.MinimumPrice, Is.EqualTo(325.50m));
            Assert.That(recipes, Has.Count.EqualTo(1));
        });
    }

    [Test, Order(4), Description("Should run through all category related methods")]
    public async Task GetRecipesByCategory() {
        List<RecipeCategory> recipeCategories = await _recipeService.GetAllRecipeCategories();
        RecipeCategory recipeCategory = await _recipeService.GetRecipeCategoryById(recipeCategories[0].Id);
        recipeCategory = await _recipeService.GetRecipeCategoryByName(recipeCategory.Name!);

        List<Recipe> recipesByCatId = await _recipeService.GetAllRecipesByCategoryId(recipeCategories[0].Id);
        List<Recipe> recipesByCatName = await _recipeService.GetAllRecipesByCategoryName(recipeCategories[0].Name!);

        Assert.That(recipesByCatId, Has.Count.EqualTo(recipesByCatName.Count));
    }

    [Test, Order(5), Description("Should Remove an ingredient from the recipe and Get all the ingredient for the recipe")]
    public async Task RemoveRecipeIngredient() {
        Recipe recipe = await _recipeService.GetRecipeById(1);
        recipe.RecipeIngredients.Remove(recipe.RecipeIngredients[1]);
        recipe = await _recipeService.UpdateRecipe(recipe);
        List<Recipe>? recipes = await _recipeService.GetAllRecipesWithCategory();

        Assert.That(recipes[0].RecipeIngredients, Has.Count.EqualTo(4));
    }

    [Test, Order(6), Description("Should add an ingredient. The method will use the UpdatedRecipe")]
    public async Task AddIngredientToRecipe() {
        Recipe recipe = await _recipeService.GetRecipeById(1);
        recipe.RecipeIngredients.Add(RecipeDataHelper.GetRecipes()[0].RecipeIngredients[1]);
        recipe = await _recipeService.UpdateRecipe(recipe);

        Assert.That(recipe.RecipeIngredients, Has.Count.EqualTo(5));
    }

    [Test, Order(7)]
    public async Task UpdateRecipe() {
        Recipe recipe = await _recipeService.GetRecipeById(1);
        recipe.FixedPrice = 400;
        await _recipeService.UpdateRecipe(recipe);

        recipe = await _recipeService.GetRecipeById(1);
        Assert.That(recipe.FixedPrice, Is.EqualTo(400));

    }

    [Test, Order(8)]
    public async Task RemoveRecipe() {
        bool recipeRemoved = await _recipeService.RemoveRecipeById(1);
        Recipe recipe = await _recipeService.GetRecipeById(1);

        Assert.That(recipe, Is.Null);
    }
}
