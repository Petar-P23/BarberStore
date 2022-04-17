using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Appointments;
using BarberStore.Core.Services;
using BarberStore.Infrastructure.Data.Enums;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BarberStore.Tests
{
    [TestFixture]
    public class AppointmentServiceTest
    {
        private ServiceProvider serviceProvider;
        private InMemoryDbContext context;
        private DateTime appointmentDate;

        [SetUp]
        public async Task Setup()
        {
            this.context = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            this.serviceProvider = serviceCollection
                .AddSingleton(sp => this.context.CreateContext())
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .AddSingleton<IAppointmentService, AppointmentService>()
                .BuildServiceProvider();

            this.appointmentDate = DateTime.Now.AddDays(10);
            var repo = this.serviceProvider.GetService<IApplicationDbRepository>();
            await SeedDbAsync(repo);
        }
        [Test]
        public async Task CreateAppointmentFailsIfModelIsNull()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            (bool success, _) = await service.CreateAppointmentAsync(null);
            Assert.That(success, Is.False);
        }
        [Test]
        public async Task CreateAppointmentFailsIfDateIsPast()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            (bool success, _) = await service.CreateAppointmentAsync(new AppointmentModel()
            {
                Start = DateTime.Now.AddDays(-10),
                UserId = "5dbd6497-f95c-403f-8818-8c1c112732f9"
            });
            Assert.That(success, Is.False);
        }
        [Test]
        public async Task CreateAppointmentFailsIfUserIdIsNull()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            (bool success, _) = await service.CreateAppointmentAsync(new AppointmentModel()
            {
                Start = DateTime.Now.AddDays(10),
                UserId = null
            });
            Assert.That(success, Is.False);
        }
        [Test]
        public async Task CreateAppointmentPassesIfModelIsCorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            (bool success, _) = await service.CreateAppointmentAsync(new AppointmentModel()
            {
                Start = DateTime.Now.AddDays(10),
                UserId = "5dbd6497-f95c-403f-8818-8c1c112732f9"
            });
            Assert.That(success, Is.True);
        }
        [Test]
        public async Task ChangeAppointmentStatusFailsIfUserIdIsNull()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            (bool success, _) =
                await service
                    .ChangeAppointmentStatusAsync(null, "2ea627fc-6df2-4713-af62-831190f332a5", Status.Cancelled);

            Assert.That(success, Is.False);
        }
        [Test]
        public async Task ChangeAppointmentStatusFailsIfAppointmentIdIsNull()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            (bool success, _) =
                await service
                    .ChangeAppointmentStatusAsync("5dbd6497-f95c-403f-8818-8c1c112732f9", null, Status.Cancelled);

            Assert.That(success, Is.False);
        }
        [Test]
        public async Task ChangeAppointmentStatusPassesIfDataIsCorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            (bool success, _) =
                await service
                    .ChangeAppointmentStatusAsync("5dbd6497-f95c-403f-8818-8c1c112732f9", "2ea627fc-6df2-4713-af62-831190f332a5", Status.Cancelled);

            Assert.That(success, Is.True);
        }
        [Test]
        public async Task CancelAppointmentPassesIfDataIsCorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            (bool success, _) =
                await service
                    .CancelAppointmentAsync("5dbd6497-f95c-403f-8818-8c1c112732f9", "2ea627fc-6df2-4713-af62-831190f332a5");

            Assert.That(success, Is.True);
        }
        [Test]
        public async Task GetAppointmentsByMonthFailsIfMonthIsInvalid()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetAllAppointmentsByMonthAsync(14), Throws.ArgumentException);
        }
        [Test]
        public async Task GetAppointmentsByMonthPassesIfMonthIsCorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetAllAppointmentsByMonthAsync(DateTime.Now.AddDays(10).Month), Is.Not.Null);
        }
        [Test]
        public async Task GetAppointmentsByUserFailsIfUserIsNull()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetUserAppointmentsAsync(null), Throws.ArgumentNullException);
        }
        [Test]
        public async Task GetAppointmentsByUserFailsIfUserIsIncorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetUserAppointmentsAsync("null"), Is.Empty);
        }
        [Test]
        public async Task GetAppointmentsByUserPassesIfUserIsCorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetUserAppointmentsAsync("5dbd6497-f95c-403f-8818-8c1c112732f9"), Is.Not.Null);
        }
        [Test]
        public async Task GetAppointmentsByDatePassesIfDateIsCorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetAllAppointmentsByDateAsync(DateTime.Now.AddDays(10)), Is.Not.Empty);
        }
        [Test]
        public async Task GetAppointmentsByDateIsEmptyIfDateIsIncorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetAllAppointmentsByDateAsync(DateTime.Now), Is.Empty);
        }
        [Test]
        public async Task CheckIfAppointmentExistsReturnsTrueIfAppointmentExists()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.CheckIfAppointmentExistsAsync(this.appointmentDate), Is.True);
        }
        [Test]
        public async Task CheckIfAppointmentExistsReturnsFalseIfAppointmentDoesNotExist()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.CheckIfAppointmentExistsAsync(DateTime.Now.AddDays(7)), Is.False);
        }
        [Test]
        public async Task GetPreviousAppointmentFailsIfWorkStartAndEndIncorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetPreviousFreeAppointmentAsync(this.appointmentDate, 0, 0), Is.Null);
        }
        [Test]
        public async Task GetPreviousAppointmentFailsIfWorkStartAndEndAreEqual()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetPreviousFreeAppointmentAsync(this.appointmentDate, 2, 2), Is.Null);
        }
        [Test]
        public async Task GetPreviousAppointmentPassesIfDataIsCorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetPreviousFreeAppointmentAsync(this.appointmentDate, 9, 18), Is.Not.Null);
        }
        [Test]
        public async Task GetNextAppointmentFailsIfWorkStartAndEndIncorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetNextFreeAppointmentAsync(this.appointmentDate, 0, 0), Is.Null);
        }
        [Test]
        public async Task GetNextAppointmentFailsIfWorkStartAndEndAreEqual()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetNextFreeAppointmentAsync(this.appointmentDate, 2, 2), Is.Null);
        }
        [Test]
        public async Task GetNextAppointmentPassesIfDataIsCorrect()
        {
            var service = this.serviceProvider.GetService<IAppointmentService>();
            Assert.That(() => service.GetNextFreeAppointmentAsync(this.appointmentDate, 9, 18), Is.Not.Null);
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
                Id = "5dbd6497-f95c-403f-8818-8c1c112732f9",
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
                Cart = new Cart() { UserId = "5dbd6497-f95c-403f-8818-8c1c112732f9" },
                CartId = default,
                Orders = null,
                Appointments = null
            };
            var appointment = new Appointment
            {
                Id = Guid.Parse("2ea627fc-6df2-4713-af62-831190f332a5"),
                Start = this.appointmentDate,
                Status = Status.Pending,
                UserId = "5dbd6497-f95c-403f-8818-8c1c112732f9"
            };

            await repo.AddAsync(user);
            await repo.AddAsync(appointment);
            await repo.SaveChangesAsync();
        }
    }
}