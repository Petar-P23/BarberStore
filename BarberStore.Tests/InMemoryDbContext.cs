using BarberStore.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BarberStore.Tests
{
    public class InMemoryDbContext
    {
        private readonly SqliteConnection connection;
        private readonly DbContextOptions<ApplicationDbContext> dbContextOptions;

        public InMemoryDbContext()
        {
            this.connection = new SqliteConnection("Filename=:memory:");
            this.connection.Open();

            this.dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(this.connection)
                .Options;

            using var context = new ApplicationDbContext(this.dbContextOptions);

            context.Database.EnsureCreated();
        }

        public ApplicationDbContext CreateContext() => new ApplicationDbContext(this.dbContextOptions);

        public void Dispose() => this.connection.Dispose();
    }
}
