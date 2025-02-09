﻿namespace FoodDesire.Web.Client.Services;
public class CartPageService : AuthorizedService, ICartPageService {
    public CartPageService(HttpClient httpClient, IAuthenticationService authenticationService)
        : base(httpClient, authenticationService) { }

    public async Task<bool> CancelOrderAsync(int orderId) {
        HttpResponseMessage response = await (await AddAuthorizationHeader()).GetAsync($"api/Cart/Cancel?orderId={orderId}");
        return response.StatusCode != HttpStatusCode.OK ? false! : await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<Order> GetOrderByIdAsync(int orderId) {
        HttpResponseMessage response = await (await AddAuthorizationHeader()).GetAsync($"api/Cart?orderId={orderId}");
        return response.StatusCode != HttpStatusCode.OK ? null! : await response.Content.ReadFromJsonAsync<Order>() ?? null!;
    }

    public async Task<Order> GetUserCurrentOrderAsync() {
        HttpResponseMessage response = await (await AddAuthorizationHeader()).GetAsync($"api/Cart/Index");
        return response.StatusCode != HttpStatusCode.OK ? null! : await response.Content.ReadFromJsonAsync<Order>() ?? null!;
    }

    public async Task<string> PayForOrderAsync(int orderId) {
        HttpResponseMessage response = await (await AddAuthorizationHeader()).PatchAsync($"api/Cart/Pay?orderId={orderId}", null);
        return response.StatusCode != HttpStatusCode.OK ? null! : await response.Content.ReadAsStringAsync() ?? null!;
    }

    public async Task<bool> RemoveFoodItemByIdAsync(int foodItemId) {
        HttpResponseMessage response = await (await AddAuthorizationHeader()).DeleteAsync($"api/Cart/RemoveFoodItem?foodItemId={foodItemId}");
        return response.StatusCode != HttpStatusCode.OK ? false! : await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<List<FoodItemListItem>> GetFoodItemsForOrderAsync(int orderId) {
        HttpResponseMessage response = await (await AddAuthorizationHeader()).GetAsync($"api/Cart/GetFoodItems?orderId={orderId}");
        return response.StatusCode != HttpStatusCode.OK ? new() : await response.Content.ReadFromJsonAsync<List<FoodItemListItem>>() ?? new();
    }

    public async Task<Order> UpdateOrderAsync(Order order) {
        HttpResponseMessage response = await (await AddAuthorizationHeader()).PatchAsJsonAsync($"api/Cart/Update", order);
        return response.StatusCode != HttpStatusCode.OK ? null! : await response.Content.ReadFromJsonAsync<Order>() ?? null!;
    }

    public async Task<Order> CompletePayment(Order order) {
        HttpResponseMessage response = await (await AddAuthorizationHeader()).PatchAsJsonAsync($"api/Cart/CompletePayment", order);
        return response.StatusCode != HttpStatusCode.OK ? null! : await response.Content.ReadFromJsonAsync<Order>() ?? null!;
    }
}
