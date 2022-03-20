using BarberStore.Core.Models;
using BarberStore.Core.Models.Store;

namespace BarberStore.Core.Contracts;

public interface IStoreService
{
    public Task<IEnumerable<ProductViewModel>> GetProductViewModels(int page, int size, string category = "");
    public Task<ProductPageViewModel?> GetProductPageViewModel(string id);
    public Task<bool> AddProductToCart(string userId, string productId, int quantity = 1);
    public Task<CartViewModel> GetCartViewModel(string userId);
    public Task<bool> PlaceOrder(PlaceOrderModel orderModel);
   // public Task<OrderViewModel> GetUserOrders()
}