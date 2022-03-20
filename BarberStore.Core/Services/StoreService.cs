using BarberStore.Core.Contracts;
using BarberStore.Core.Models;
using BarberStore.Core.Models.Store;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberStore.Core.Services;

public class StoreService : IStoreService
{
    private readonly IApplicationDbRepository repo;

    public StoreService(IApplicationDbRepository repo)
    {
        this.repo = repo;
    }

    public async Task<IEnumerable<ProductViewModel>> GetProductViewModels(int page, int size, string category = "")
    {
        bool noFilter = string.IsNullOrEmpty(category);

        return await repo.All<Product>()
            .Where(p => p.Category.Name == category || noFilter)
            .OrderBy(p => p.Price)
            .Skip(page * size)
            .Take(size)
            .Select(p => new ProductViewModel
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Price = p.Price
            }).ToListAsync();
    }

    public async Task<ProductPageViewModel?> GetProductPageViewModel(string id)
    {
        return await repo.All<Product>()
            .Where(p => p.Id.ToString() == id)
            .Select(p => new ProductPageViewModel
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Price = p.Price,
                Description = p.Description
            })
            .SingleOrDefaultAsync();
    }

    public async Task<bool> AddProductToCart(string userId, string productId, int quantity = 1)
    {
        try
        {
            var user = await repo.GetByIdAsync<ApplicationUser>(userId);
            var cart = user.Cart ?? new Cart() { User = user };

            var product = await repo.GetByIdAsync<Product>(productId);

            cart.CartProducts.Add(new CartProduct
            {
                Product = product,
                Quantity = quantity
            });

            await repo.AddAsync(cart);
            await repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public async Task<CartViewModel> GetCartViewModel(string userId)
    {
        return (await repo.All<Cart>()
            .Where(c => c.UserId.ToString() == userId)
            .Select(c => new CartViewModel
            {
                Id = c.Id.ToString(),
                Products = c.CartProducts
                    .Where(cp => cp.Ordered == false)
                    .Select(cp => new CartProductViewModel
                {
                    Id = cp.Product.Id.ToString(),
                    Name = cp.Product.Name,
                    Price = cp.Product.Price,
                    Quantity = cp.Quantity,
                    ImagePath = cp.Product.ImagePath
                })
                    .ToList()
            }).SingleOrDefaultAsync())!;
    }

    public async Task<bool> PlaceOrder(PlaceOrderModel orderModel)
    {
        try
        {
            var order = new Order
            {
                UserId = orderModel.UserId,
                TimeOfOrdering = DateTime.Now.Date,
                Address = orderModel.Address,
                OrderProducts = orderModel.ProductIds
                    .Select(p => new OrderProduct()
                    {
                        ProductId = Guid.Parse(p)
                    }).ToList()
            };

            await repo.AddAsync(order);
            await repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}