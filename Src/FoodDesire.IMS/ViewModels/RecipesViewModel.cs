﻿using CommunityToolkit.Mvvm.Input;

namespace FoodDesire.IMS.ViewModels;
public partial class RecipesViewModel : ObservableRecipient, INavigationAware {
    private readonly INavigationService _navigationService;
    private readonly IRecipesPageService _recipesPageService;
    private readonly IMapper _mapper;

    public bool IsChef => App.CurrentUserAccount?.Role == Role.Chef;
    [ObservableProperty]
    private bool _isLoading = true;
    private string? _searchText;
    public string? SearchText {
        get => _searchText;
        set {
            OnSearchTextChanged();
            SetProperty(ref _searchText, value);
        }
    }
    private readonly SemaphoreSlim _searchSemaphore = new(1, 1);

    public ObservableCollection<RecipeListItemDetail> Recipes { get; } = new();
    public IRelayCommand<RecipeListItemDetail> OnItemClickedCommand { get; }

    public RecipesViewModel(INavigationService navigationService, IRecipesPageService recipesPageService, IMapper mapper) {
        _navigationService = navigationService;
        _recipesPageService = recipesPageService;
        _mapper = mapper;
        OnItemClickedCommand = new RelayCommand<RecipeListItemDetail>(OnItemClick);
    }

    public void OnNavigatedTo(object parameter) {
        OnSearchTextChanged();
    }

    public void OnNavigatedFrom() {
    }

    private async void OnSearchTextChanged() {
        await _searchSemaphore.WaitAsync();
        try {
            IsLoading = true;
            if (string.IsNullOrEmpty(SearchText) || string.IsNullOrWhiteSpace(SearchText)) {
                Recipes.Clear();
                List<Recipe> recipes = await _recipesPageService.GetAllRecipes();
                recipes.ForEach(e => Recipes.Add(_mapper.Map<RecipeListItemDetail>(e)));
            } else {
                Recipes.Clear();
                List<Recipe> recipes = await _recipesPageService.SearchRecipes(SearchText);
                recipes.ForEach(e => Recipes.Add(_mapper.Map<RecipeListItemDetail>(e)));
            }
            IsLoading = false;
        } finally {
            _searchSemaphore.Release();
        }
    }

    public void OnItemClick(RecipeListItemDetail? clickedItem) {
        if (clickedItem != null) {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(RecipesDetailViewModel).FullName!, clickedItem.Id);
        }
    }

    [RelayCommand]
    public void AddNewRecipe(XamlRoot xamlRoot) {
        _navigationService.NavigateTo(typeof(NewRecipeViewModel).FullName!);
    }
}
