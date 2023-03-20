using Microsoft.EntityFrameworkCore;
namespace RM.Api.Data
{
    public class AppDbContextFactory : IAppDbContextFactory
    {
        private readonly Func<DbContextOptions<AppDbContext>> _optionsFactory;

        public AppDbContextFactory(Func<DbContextOptions<AppDbContext>> optionsFactory)
        {
            _optionsFactory = optionsFactory;
        }

        public AppDbContext CreateDbContext()
        {
            return new AppDbContext(_optionsFactory());
        }
    }
}
