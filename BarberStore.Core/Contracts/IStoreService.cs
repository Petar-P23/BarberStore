using BarberStore.Core.Models.Store;
using BarberStore.Infrastructure.Data.Enums;
using BarberStore.Infrastructure.Data.Models;
using System.Linq.Expressions;

namespace BarberStore.Core.Contracts;

public interface IStoreService
{
    /// <summary>
    /// Used to get a store page with its products.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="size">The number of products to be displayed in a page.</param>
    /// <param name="category">Filters products by category.</param>
    /// <returns>Returns a store page view model.</returns>
    public Task<StorePageViewModel> GetStorePageAsync(int page, int size, string category = "");

    public Task<StorePageViewModel> GetStorePageAsync(int page, int size,
        Expression<Func<Product, object>> orderByExpression, string category = "");
    /// <summary>
    /// Used to get the number of all pages.
    /// </summary>
    /// <param name="size">The size of the page</param>
    /// <param name="filterExpression">An expression used to filter the products.</param>
    /// <returns>Returns the number of all pages.</returns>
    public Task<int> GetProductPagesCountAsync(int size, Expression<Func<Product, bool>> filterExpression);
    /// <summary>
    /// Used to get a given product model.
    /// </summary>
    /// <param name="productId">The product id.</param>
    /// <returns>Returns a product page view model.</returns>
    public Task<ProductPageViewModel?> GetProductPageAsync(string productId);
    /// <summary>
    /// Used to add a product to a user's cart.
    /// </summary>
    /// <param name="userId">The user who owns the cart.</param>
    /// <param name="productId">The product to be added.</param>
    /// <param name="quantity">The quantity of the product to be added.</param>
    /// <returns>Returns true if adding was successful. Returns false and a string with an error message if it was unsuccessful.</returns>
    public Task<(bool, string)> AddProductToCartAsync(string userId, string productId, int quantity = 1);
    /// <summary>
    /// Used to get a user's cart.
    /// </summary>
    /// <param name="userId">The user who owns the cart.</param>
    /// <returns>Returns a cart view model with products.</returns>
    public Task<CartViewModel> GetCartAsync(string userId);
    /// <summary>
    /// Used to create a new order.
    /// </summary>
    /// <returns>Returns true if adding was successful. Returns false and a string with an error message if it was unsuccessful.</returns>
    public Task<(bool, string)> PlaceOrderAsync(PlaceOrderProductModel[]? products, string userId);
    /// <summary>
    /// Used to get all of a given user's orders.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Returns a collection of order view models.</returns>
    public Task<IEnumerable<OrderViewModel>> GetOrdersByUserAsync(string userId);
    /// <summary>
    /// Used to get all orders filtered by their status.
    /// </summary>
    /// <param name="status"></param>
    /// <returns>Returns a collection of order view models.</returns>
    public Task<IEnumerable<OrderViewModel>> GetAllOrdersByStatusAsync(Status status);

    public Task MarkOrderAsFinisedAsync(string orderId);
    Task<bool> CreateNewProductAsync(string name, string imageName, decimal price, string description);
    Task<bool> CreateNewProductAsync(string name, string imageName, decimal price, string description, string categoryName);
    Task<bool> RemoveProductAsync(string id);
    Task<bool> RemoveFromCartAsync(string userId, string productId);
}