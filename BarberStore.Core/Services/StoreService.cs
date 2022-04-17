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
    public async Task<StorePageViewModel> GetStorePageAsync(int page, int size, string category = "")
    {
        return await this.GetStorePageAsync(page, size, p => p.Price, category);
    }

    public async Task<StorePageViewModel> GetStorePageAsync(int page, int size, Expression<Func<Product, object>> orderByExpression, string category = "")
    {
        var noFilter = string.IsNullOrWhiteSpace(category);
        var pageCount = await this.GetProductPagesCountAsync(size, p => p.Category.Name == category || noFilter);
        if (page >= pageCount) page = 0;

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

        if (pageCount == 0) pageCount = 1;

        return new StorePageViewModel
        {
            PageNumber = page + 1,
            PagesCount = pageCount,
            Products = products
        };
    }

    public async Task<int> GetProductPagesCountAsync(int size, Expression<Func<Product, bool>> filterExpression)
    {
        Guard.AgainstNull(filterExpression, nameof(filterExpression));
        if (size == 0) return 0;

        return (int)Math.Ceiling(await this.repo.All<Product>()
            .Where(filterExpression)
            .CountAsync() / (double)size);
    }

    public async Task<ProductPageViewModel?> GetProductPageAsync(string productId)
    {
        Guard.AgainstNullOrWhiteSpaceString(productId, nameof(productId));
        Guid id;
        try
        {
            id = Guid.Parse(productId);
        }
        catch (Exception)
        {
            return null;
        }

        var product = await this.repo.All<Product>()
            .Where(p => p.Id == id)
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

    public async Task<(bool, string)> AddProductToCartAsync(string? userId, string? productId, int quantity = 1)
    {
        if (quantity <= 0)
        {
            return (false, "Quantity cannot be zero.");
        }
        try
        {
            var user = await this.repo.All<ApplicationUser>()
                .Where(u => u.Id == userId)
                .Include(u => u.Cart)
                .SingleOrDefaultAsync();

            Guard.AgainstNull(user, nameof(user));
            if (user.Cart == null)
            {
                user.Cart = new Cart { UserId = userId };
            }

            var product = await this.repo.GetByIdAsync<Product>(Guid.Parse(productId));
            Guard.AgainstNull(product, nameof(product));

            var cp = new CartProduct
            {
                Cart = user.Cart,
                Product = product,
                Quantity = quantity
            };

            await this.repo.AddAsync(cp);
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return (false, UnexpectedErrorMessage);
        }

        return (true, string.Empty);
    }

    public async Task<CartViewModel> GetCartAsync(string? userId)
    {
        Guard.AgainstNullOrWhiteSpaceString(userId, nameof(userId));

        var cart = await this.repo.All<Cart>()
            .Where(c => c.UserId == userId)
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

        if (!await this.repo.All<ApplicationUser>().AnyAsync(u => u.Id == userId)) return null;

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

    public async Task<(bool, string)> PlaceOrderAsync(PlaceOrderProductModel[]? products, string userId)
    {
        try
        {
            Guard.AgainstNull(products, nameof(products));
            Guard.AgainstNullOrWhiteSpaceString(userId, nameof(userId));
            if (products.Any(p => p.Quantity <= 0)) return (false, "Quantity cannot be less than 1.");

            var order = new Order
            {
                UserId = userId,
                TimeOfOrdering = DateTime.Now,
                Address = "",
                Status = Status.Pending,
                OrderProducts = products
                    .Select(p => new OrderProduct
                    {
                        ProductId = Guid.Parse(p.Id),
                        Quantity = p.Quantity
                    }).ToList()
            };
            var cartId = await
                this.repo.All<Cart>()
                    .Where(c => c.UserId == userId)
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();

            var cartProducts = await this.repo.All<CartProduct>()
                .Where(cp => cp.CartId == cartId
                             && products.Select(p => p.Id).Contains(cp.ProductId.ToString())
                             && cp.Ordered == false)
                .ToListAsync();

            foreach (var product in cartProducts)
            {
                if (product != null)
                    product.Ordered = true;
            }

            await this.repo.AddAsync(order);
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return (false, UnexpectedErrorMessage);
        }

        return (true, string.Empty);
    }

    public async Task<IEnumerable<OrderViewModel>> GetOrdersByUserAsync(string userId)
    {
        Guard.AgainstNullOrWhiteSpaceString(userId, nameof(userId));

        if (!await this.repo.All<ApplicationUser>().AnyAsync(u => u.Id == userId)) return null;

        return await this.repo.All<Order>()
            .Where(o => o.User.Id == userId)
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
                }).ToList()
            })
            .OrderByDescending(o => o.Status)
            .ThenByDescending(o => o.OrderTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderViewModel>> GetAllOrdersByStatusAsync(Status status)
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
                }).ToList()
            })
            .OrderByDescending(o => o.OrderTime)
            .ToListAsync();
    }

    public async Task MarkOrderAsFinisedAsync(string orderId)
    {
        Guard.AgainstNullOrWhiteSpaceString(orderId, nameof(orderId));
        var order = await this.repo.All<Order>()
            .Where(o => o.Id == Guid.Parse(orderId))
            .FirstOrDefaultAsync();
        Guard.AgainstNull(order, nameof(order));

        order.Status = Status.Finished;
        await this.repo.SaveChangesAsync();
    }

    public async Task<bool> CreateNewProductAsync(string name, string imageName, decimal price, string description)
     => await CreateNewProductAsync(name, imageName, price, description, "General");

    public async Task<bool> CreateNewProductAsync(string name, string imageName, decimal price, string description,
        string categoryName)
    {
        if (price <= 0) return false;
        try
        {
            var category = await this.repo.All<Category>()
                .FirstOrDefaultAsync(c => c.Name == categoryName) ?? new Category { Name = categoryName };

            var product = new Product
            {
                Name = name,
                Description = description,
                ImagePath = imageName,
                Price = price,
                Category = category,
            };

            await this.repo.AddAsync(product);
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> RemoveProductAsync(string id)
    {
        try
        {
            Guard.AgainstNullOrWhiteSpaceString(id);
            await this.repo.DeleteAsync<Product>(Guid.Parse(id));
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> RemoveFromCartAsync(string userId, string productId)
    {
        try
        {
            Guard.AgainstNullOrWhiteSpaceString(userId, nameof(userId));
            Guard.AgainstNullOrWhiteSpaceString(productId, nameof(productId));

            var cp = await this.repo.All<CartProduct>()
                .Where(cp => cp.ProductId == Guid.Parse(productId) && cp.Cart.UserId == userId && cp.Ordered == false)
                .FirstOrDefaultAsync();
            Guard.AgainstNull(cp, "Product");

            await this.repo.DeleteAsync<CartProduct>(cp.Id);
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}