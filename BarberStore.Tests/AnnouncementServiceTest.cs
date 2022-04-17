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
public class AnnouncementServiceTest
{
    private ServiceProvider serviceProvider;
    private InMemoryDbContext context;
    private string userId = "5dbd6497-f95c-403f-8818-8c1c112732f9";
    private string announcementId = "5dbd6497-f95c-403f-8818-8c1c112732f9";
    private IAnnouncementService service;

    [SetUp]
    public async Task Setup()
    {
        this.context = new InMemoryDbContext();
        var serviceCollection = new ServiceCollection();

        this.serviceProvider = serviceCollection
            .AddSingleton(sp => this.context.CreateContext())
            .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
            .AddSingleton<IAnnouncementService, AnnouncementService>()
            .BuildServiceProvider();

        this.service = this.serviceProvider.GetService<IAnnouncementService>();
        var repo = this.serviceProvider.GetService<IApplicationDbRepository>();
        await SeedDbAsync(repo);
    }

    [Test]
    public async Task CreateAnnouncementFailsIfTextIsNullOrEmpty()
    {
        var (success, _) = await this.service.CreateAnnouncementAsync(null);
        Assert.That(success, Is.False);
        (success, _) = await this.service.CreateAnnouncementAsync("");
        Assert.That(success, Is.False);
    }

    [Test]
    public async Task CreateAnnouncementPassesIfTextIsNotNullOrEmpty()
    {
        var (success, _) = await this.service.CreateAnnouncementAsync("Test");
        Assert.That(success, Is.True);
    }

    [Test]
    public async Task GetAnnouncementsMethodsPassIfThereAreAnnouncements()
    {
        Assert.That(await this.service.GetAllAnnouncementsAsync(), Is.Not.Empty);
        Assert.That(await this.service.GetAllAnnouncementsAsync(1), Is.Not.Empty);
        Assert.That(await this.service.GetAllAnnouncementsAsync(1, 0), Is.Not.Empty);
        Assert.That(await this.service.GetTopAnnouncementsAsync(), Is.Not.Empty);
    }

    [Test]
    public async Task GetAnnouncementsMethodsFailIfNegativeParametersArePassed()
    {
        Assert.That(await this.service.GetAllAnnouncementsAsync(-1), Is.Null);
        Assert.That(await this.service.GetAllAnnouncementsAsync(-1, 0), Is.Null);
        Assert.That(await this.service.GetAllAnnouncementsAsync(1, -1), Is.Null);
    }

    [Test]
    public async Task RemoveAnnouncementFailsIfIdIsNullEmptyOrWrong()
    {
        Assert.That(await this.service.RemoveAnnouncementAsync(null), Is.False);
        Assert.That(await this.service.RemoveAnnouncementAsync(""), Is.False);
        Assert.That(await this.service.RemoveAnnouncementAsync("null"), Is.False);
        Assert.That(await this.service.RemoveAnnouncementAsync("5db46497-f95c-403f-8818-8c1c112732f9"), Is.False);
    }

    [Test]
    public async Task RemoveAnnouncementPassesIfAnnouncementExists()
    {
        Assert.That(await this.service.RemoveAnnouncementAsync(this.announcementId), Is.True);
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
        var announcement = new Announcement
        {
            Id = Guid.Parse(this.announcementId),
            MainText = "null",
            PublishDate = DateTime.Now
        };
        await repo.AddAsync(announcement);
        await repo.AddAsync(user);
        await repo.SaveChangesAsync();
    }
}