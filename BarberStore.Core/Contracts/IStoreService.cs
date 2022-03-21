using BarberStore.Core.Models.Store;
using BarberStore.Infrastructure.Data.Enums;
using BarberStore.Infrastructure.Data.Models;
using System.Linq.Expressions;

namespace BarberStore.Core.Contracts;

public interface IStoreService
{
    public Task<StorePageViewModel> GetStorePage(int page, int size, string category = "");
    public Task<int> GetProductPagesCount(int size, Expression<Func<Product, bool>> filterExpression);
    public Task<ProductPageViewModel?> GetProductPage(string productId);
    public Task<bool> AddProductToCart(string userId, string productId, int quantity = 1);
    public Task<CartViewModel> GetCart(string userId);
    public Task<bool> PlaceOrder(PlaceOrderModel orderModel);
    public Task<IEnumerable<OrderViewModel>> GetOrdersByUser(string userId);
    public Task<IEnumerable<OrderViewModel>> GetAllOrdersByStatus(Status status);
}