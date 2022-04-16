using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Store;
using BarberStore.Core.Services;
using BarberStore.Infrastructure.Data.Enums;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BarberStore.Tests;

[TestFixture]
public class StoreServiceTest
{
    private ServiceProvider serviceProvider;
    private InMemoryDbContext context;
    private string userId = "5dbd6497-f95c-403f-8818-8c1c112732f9";
    private string productId = "b6f4b3d2-da59-45c4-a52a-cff9089acd80";
    private string orderId = "5d94d83b-dbdf-457b-9995-634d4a833d58";

    [SetUp]
    public async Task Setup()
    {
        this.context = new InMemoryDbContext();
        var serviceCollection = new ServiceCollection();

        this.serviceProvider = serviceCollection
            .AddSingleton(sp => this.context.CreateContext())
            .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
            .AddSingleton<IStoreService, StoreService>()
            .BuildServiceProvider();

        var repo = this.serviceProvider.GetService<IApplicationDbRepository>();
        await SeedDbAsync(repo);
    }

    [Test]
    public async Task GetStorePageAsyncReturnsEmptyIfSizeIsZero()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var productPage = await service.GetStorePageAsync(0, 0, x => x);
        Assert.That(productPage.Products, Is.Empty);
        Assert.That(productPage.PageNumber, Is.EqualTo(1));
        Assert.That(productPage.PagesCount, Is.EqualTo(1));
    }
    [Test]
    public async Task GetStorePageAsyncThrowsIfOrderByIsNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.GetStorePageAsync(0, 0, null, ""), Throws.ArgumentNullException);
    }
    [Test]
    public async Task GetStorePageAsyncPassesIfCorrectInput()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var productPage = await service.GetStorePageAsync(0, 1, x => x);
        Assert.That(productPage.Products, Is.Not.Empty);
    }

    [Test]
    public async Task GetProductPageThrowsIfProductIdIsNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();

        Assert.That(() => service.GetProductPageAsync(null), Throws.ArgumentNullException);
    }

    [Test]
    public async Task GetProductPageThrowsIfProductIdIsEmpty()
    {
        var service = this.serviceProvider.GetService<IStoreService>();

        Assert.That(() => service.GetProductPageAsync(""), Throws.ArgumentNullException);
    }

    [Test]
    public async Task GetProductPageReturnsNullIfProductIdIsIncorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();

        Assert.That(() => service.GetProductPageAsync("b6f4b3d2-dc59-45c4-a52a-cff9089acd80"), Is.Null);
    }

    [Test]
    public async Task GetProductPageReturnsProductIfProductIdIsCorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();

        Assert.That(() => service.GetProductPageAsync("b6f4b3d2-da59-45c4-a52a-cff9089acd80"), Is.Not.Null);
    }

    [Test]
    public async Task AddProductToCartFailsIfUserIsNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var (success, _) = await service.AddProductToCartAsync(null, this.productId);
        Assert.That(success, Is.False);
    }

    [Test]
    public async Task AddProductToCartFailsIfProductIsNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var (success, _) = await service.AddProductToCartAsync(this.userId, null);
        Assert.That(success, Is.False);
    }

    [Test]
    public async Task AddProductToCartFailsIfQuantityIsZero()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var (success, _) = await service.AddProductToCartAsync(this.userId, this.productId, 0);
        Assert.That(success, Is.False);
    }

    [Test]
    public async Task AddProductToCartPassesIfDataIsCorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var (success, _) = await service.AddProductToCartAsync(this.userId, this.productId, 1);
        Assert.That(success, Is.True);
    }

    [Test]
    public async Task GetCartThrowsIfUserIsNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.GetCartAsync(null), Throws.ArgumentNullException);
    }

    [Test]
    public async Task GetCartFailsIfUserIsIncorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.GetCartAsync("null"), Is.Null);
    }

    [Test]
    public async Task GetCartPassesIfUserIsCorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.GetCartAsync(this.userId), Is.Not.Null);
    }

    [Test]
    public async Task PlaceOrderFailsIfNoProductsArePassed()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var (success, _) = await service.PlaceOrderAsync(null, this.userId);
        Assert.That(success, Is.False);
    }

    [Test]
    public async Task PlaceOrderFailsIfUserIsNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var products = new PlaceOrderProductModel[]{
            new PlaceOrderProductModel()
        {
            Id = this.productId,
            Quantity = 1
        }};
        var (success, _) = await service.PlaceOrderAsync(products, null);
        Assert.That(success, Is.False);
    }

    [Test]
    public async Task PlaceOrderFailsIfUserIsWrong()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var products = new PlaceOrderProductModel[]{
            new PlaceOrderProductModel()
            {
                Id = this.productId,
                Quantity = 1
            }};
        var (success, _) = await service.PlaceOrderAsync(products, "null");
        Assert.That(success, Is.False);
    }

    [Test]
    public async Task PlaceOrderFailsIfQuantityIsZero()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var products = new PlaceOrderProductModel[]{
            new PlaceOrderProductModel()
            {
                Id = this.productId,
                Quantity = 0
            }};
        var (success, _) = await service.PlaceOrderAsync(products, this.userId);
        Assert.That(success, Is.False);
    }

    [Test]
    public void GetOrdersByUserThrowsIfUserIsNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(()=>service.GetOrdersByUserAsync(null),Throws.ArgumentNullException);
    }

    [Test]
    public void GetOrdersByUserThrowsIfUserIsEmpty()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.GetOrdersByUserAsync(""), Throws.ArgumentNullException);
    }

    [Test]
    public void GetOrdersByUserReturnsNullIfUserIsIncorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.GetOrdersByUserAsync("incorrect"), Is.Null);
    }

    [Test]
    public void GetOrdersByUserPassesIfUserIsCorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.GetOrdersByUserAsync(this.userId), Is.Not.Null);
    }

    [Test]
    public async Task PlaceOrderPassesIfDataIsCorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        var products = new PlaceOrderProductModel[]{
            new PlaceOrderProductModel()
            {
                Id = this.productId,
                Quantity = 1
            }};
        var (success, _) = await service.PlaceOrderAsync(products, this.userId);
        Assert.That(success, Is.True);
    }

    [Test]
    public void GetOrdersByStatusReturnsEmptyIfNoOrders()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.GetAllOrdersByStatusAsync(Status.Cancelled), Is.Empty);
    }

    [Test]
    public void GetOrdersByStatusReturnsNotEmptyIfOrdersExist()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.GetAllOrdersByStatusAsync(Status.Pending), Is.Not.Empty);
    }

    [Test]
    public void MarkOrderAsFinishedThrowsIfOrderIsNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.MarkOrderAsFinisedAsync(null), Throws.ArgumentNullException);
    }

    [Test]
    public void MarkOrderAsFinishedThrowsIfOrderIsEmpty()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.MarkOrderAsFinisedAsync(""), Throws.ArgumentNullException);
    }

    [Test]
    public void MarkOrderAsFinishedThrowsIfOrderIsWrong()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.MarkOrderAsFinisedAsync(this.userId), Throws.ArgumentNullException);
    }

    [Test]
    public void MarkOrderAsFinishedPassesIfOrderIsCorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.MarkOrderAsFinisedAsync(this.orderId), Throws.Nothing);
    }

    [Test]
    public void CreateProductFailsIfAnyParameterIsNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.CreateNewProductAsync(null,"test",5,"test"), Is.False);
        Assert.That(() => service.CreateNewProductAsync("null", null, 5, "test"), Is.False);
        Assert.That(() => service.CreateNewProductAsync("null", "test", 0, "test"), Is.False);
        Assert.That(() => service.CreateNewProductAsync("null", "test", 5, null), Is.False);
    }

    [Test]
    public void CreateProductFailsIfDataIsCorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.CreateNewProductAsync("null", "test", 5, "test"), Is.True);
    }

    [Test]
    public void RemoveProductFailsIfProductIsIncorrectOrNull()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.RemoveProductAsync(null), Is.False);
        Assert.That(() => service.RemoveProductAsync("null"), Is.False);
    }

    [Test]
    public void RemoveProductPassesIfProductIsCorrect()
    {
        var service = this.serviceProvider.GetService<IStoreService>();
        Assert.That(() => service.RemoveProductAsync(this.productId), Is.True);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private async Task SeedDbAsync(IApplicationDbRepository repo)
    {
        var user = new ApplicationUser
        {
            Id = this.userId,
            UserName = "Test",
            NormalizedUserName = "Test",
            Email = "Test@test.test",
            NormalizedEmail = "Test@test.test",
            EmailConfirmed = false,
            PasswordHash = "Test@test.test",
            SecurityStamp = null,
            ConcurrencyStamp = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            FirstName = "Test",
            LastName = "Test",
            Cart = new Cart() { UserId = this.userId },
            CartId = default,
            Orders = null,
            Appointments = null
        };

        var product = new Product
        {
            Id = Guid.Parse(this.productId),
            Name = "test",
            Description = "test",
            ImagePath = "test",
            Price = 4,
            Category = new Category()
            {
                Name = "test"
            }
        };

        var order = new Order
        {
            Id = Guid.Parse(this.orderId),
            UserId = this.userId,
            TimeOfOrdering = DateTime.Now,
            Address = "test",
            Status = Status.Pending,
            OrderProducts = new List<OrderProduct>()
            {
                new() { ProductId = Guid.Parse(this.productId) }
            }
        };

        await repo.AddAsync(product);
        await repo.AddAsync(user);
        await repo.AddAsync(order);
        await repo.SaveChangesAsync();
    }
}