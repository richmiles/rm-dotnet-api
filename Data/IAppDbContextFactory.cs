namespace RM.Api.Data
{
    public interface IAppDbContextFactory
    {
        AppDbContext CreateDbContext();
    }
}
