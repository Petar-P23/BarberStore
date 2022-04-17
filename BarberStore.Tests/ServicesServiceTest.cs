using BarberStore.Core.Contracts;
using BarberStore.Core.Services;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BarberStore.Tests;

[TestFixture]
public class ServicesServiceTest
{
    private ServiceProvider serviceProvider;
    private InMemoryDbContext context;
    private IServicesService service;
    private string serviceId = "5dbd6497-f95c-403f-8818-8c1c112732f9";

    [SetUp]
    public async Task Setup()
    {
        this.context = new InMemoryDbContext();
        var serviceCollection = new ServiceCollection();

        this.serviceProvider = serviceCollection
            .AddSingleton(sp => this.context.CreateContext())
            .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
            .AddSingleton<IServicesService, ServicesService>()
            .BuildServiceProvider();

        this.service = this.serviceProvider.GetService<IServicesService>();
        var repo = this.serviceProvider.GetService<IApplicationDbRepository>();
        await SeedDbAsync(repo);
    }

    [Test]
    public async Task CreateServiceFailsIfAnyParameterIsInvalid()
    {
        Assert.That(await this.service.CreateServiceAsync(null, "desc", 5), Is.False);
        Assert.That(await this.service.CreateServiceAsync("null", "", 5), Is.False);
        Assert.That(await this.service.CreateServiceAsync("null", null, 5), Is.False);
        Assert.That(await this.service.CreateServiceAsync("", "desc", 5), Is.False);
        Assert.That(await this.service.CreateServiceAsync("null", "desc", 0), Is.False);
        Assert.That(await this.service.CreateServiceAsync("null", "desc", -5), Is.False);
    }

    [Test]
    public async Task CreateServicePassesIfDataIsCorrect()
    {
        Assert.That(await this.service.CreateServiceAsync("test", "test", 5.5m), Is.True);
    }

    [Test]
    public async Task RemoveServiceFailsIfIdIsInvalid()
    {
        Assert.That(await this.service.RemoveServiceAsync(""), Is.False);
        Assert.That(await this.service.RemoveServiceAsync(null), Is.False);
        Assert.That(await this.service.RemoveServiceAsync("null"), Is.False);
        Assert.That(await this.service.RemoveServiceAsync("5dbd6497-f95c-403f-8818-8c1c312732f9"), Is.False);
    }

    [Test]
    public async Task RemoveServicePassesIfIdIsValid()
    {
        Assert.That(await this.service.RemoveServiceAsync(this.serviceId), Is.True);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private async Task SeedDbAsync(IApplicationDbRepository repo)
    {
        var service = new Service
        {
            Id = Guid.Parse(this.serviceId),
            Name = "test",
            Price = 5,
            Description = "test"
        };

        await repo.AddAsync(service);
        await repo.SaveChangesAsync();
    }
}