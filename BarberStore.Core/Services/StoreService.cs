using BarberStore.Core.Common;
using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Store;
using BarberStore.Infrastructure.Data.Enums;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static BarberStore.Core.Constants.ExceptionMessageConstants;

namespace BarberStore.Core.Services;

public class StoreService : DataService, IStoreService
{
    public StoreService(IApplicationDbRepository repo)
         : base(repo)
    {
    }
    public async Task<StorePageViewModel> GetStorePage(int page, int size, string category = "")
    {
        return await this.GetStorePage(page, size, p => p.Price, category);
    }

    public async Task<StorePageViewModel> GetStorePage(int page, int size, Expression<Func<Product, object>> orderByExpression, string category = "")
    {
        var noFilter = string.IsNullOrWhiteSpace(category);
        var pageCount = await this.GetProductPagesCount(size, p => p.Category.Name == category || noFilter);
        if (page > pageCount) page = 0;

        Guard.AgainstNull(orderByExpression, nameof(orderByExpression));

        var products = await this.repo.All<Product>()
            .Where(p => p.Category.Name == category || noFilter)
            .OrderBy(orderByExpression)
            .Skip(page * size)
            .Take(size)
            .Select(p => new ProductViewModel
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Price = p.Price,
                Image = p.ImagePath
            }).ToListAsync();

        return new StorePageViewModel
        {
            PageNumber = page + 1,
            PagesCount = pageCount,
            Products = products
        };
    }

    public async Task<int> GetProductPagesCount(int size, Expression<Func<Product, bool>> filterExpression)
    {
        Guard.AgainstNull(filterExpression, nameof(filterExpression));

        return (int)Math.Ceiling(await this.repo.All<Product>()
            .Where(filterExpression)
            .CountAsync() / (double)size);
    }

    public async Task<ProductPageViewModel?> GetProductPage(string productId)
    {
        Guard.AgainstNullOrWhiteSpaceString(productId, nameof(productId));

        var product = await this.repo.All<Product>()
            .Where(p => p.Id.ToString() == productId)
            .Select(p => new ProductPageViewModel
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Image = p.ImagePath
            })
            .SingleOrDefaultAsync();

        return product;
    }

    public async Task<(bool, string)> AddProductToCart(string? userId, string? productId, int quantity = 1)
    {
        try
        {
            var user = await this.repo.GetByIdAsync<ApplicationUser>(userId);
            Guard.AgainstNull(user, nameof(user));

            var cart = user.Cart ?? new Cart { User = user };

            var product = await this.repo.GetByIdAsync<Product>(productId);
            Guard.AgainstNull(product, nameof(product));

            cart.CartProducts.Add(new CartProduct
            {
                Product = product,
                Quantity = quantity
            });

            await this.repo.AddAsync(cart);
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return (false, UnexpectedErrorMessage);
        }

        return (true, string.Empty);
    }

    public async Task<CartViewModel> GetCart(string? userId)
    {
        Guard.AgainstNullOrWhiteSpaceString(userId, nameof(userId));

        var cart = await this.repo.All<Cart>()
            .Where(c => c.UserId!.ToString() == userId)
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
            }).SingleOrDefaultAsync();

        if (cart != null) return cart;

        var cartId = Guid.NewGuid();
        var newCart = new Cart
        {
            Id = cartId,
            UserId = userId
        };
        await this.repo.AddAsync(newCart);
        await this.repo.SaveChangesAsync();

        return new CartViewModel() { Id = cartId.ToString(), Products = new List<CartProductViewModel>() };

    }

    public async Task<(bool, string)> PlaceOrder(PlaceOrderModel orderModel)
    {
        try
        {
            Guard.AgainstNull(orderModel, nameof(orderModel));

            var order = new Order
            {
                UserId = orderModel.UserId,
                TimeOfOrdering = DateTime.Now.Date,
                Address = orderModel.Address,
                OrderProducts = orderModel.ProductIds
                    .Select(p => new OrderProduct
                    {
                        ProductId = Guid.Parse(p)
                    }).ToList()
            };

            await this.repo.AddAsync(order);
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return (false, UnexpectedErrorMessage);
        }

        return (true, string.Empty);
    }

    public async Task<IEnumerable<OrderViewModel>> GetOrdersByUser(string userId)
    {
        Guard.AgainstNullOrWhiteSpaceString(userId, nameof(userId));

        return await this.repo.All<Order>()
            .Where(o => o.User.Id.ToString() == userId)
            .Select(o => new OrderViewModel
            {
                Id = o.Id.ToString(),
                UserFirstName = o.User.FirstName,
                UserLastName = o.User.LastName,
                OrderTime = o.TimeOfOrdering,
                Status = o.Status,
                Products = o.OrderProducts.Select(op => new OrderProductViewModel
                {
                    Id = op.Product.Id.ToString(),
                    Name = op.Product.Name,
                    Price = op.Product.Price,
                    Quantity = op.Quantity,
                    ImagePath = op.Product.ImagePath
                }).ToList()
            })
            .OrderByDescending(o => o.Status)
            .ThenByDescending(o => o.OrderTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderViewModel>> GetAllOrdersByStatus(Status status)
    {
        Guard.AgainstNull(status, nameof(status));
        return await this.repo.All<Order>()
            .Where(o => o.Status == status)
            .Select(o => new OrderViewModel
            {
                Id = o.Id.ToString(),
                UserFirstName = o.User.FirstName,
                UserLastName = o.User.LastName,
                OrderTime = o.TimeOfOrdering,
                Status = o.Status,
                Products = o.OrderProducts.Select(op => new OrderProductViewModel
                {
                    Id = op.Product.Id.ToString(),
                    Name = op.Product.Name,
                    Price = op.Product.Price,
                    Quantity = op.Quantity,
                    ImagePath = op.Product.ImagePath
                }).ToList()
            })
            .OrderByDescending(o => o.OrderTime)
            .ToListAsync();
    }
}