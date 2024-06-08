using Microsoft.EntityFrameworkCore;

namespace ScadaPlus.Data.Contexts;

public class ScadaPlusDbContextFactory
{
    private readonly DbContextOptions _options;
    public ScadaPlusDbContextFactory(DbContextOptions options)
    {
        _options = options;
    }

    public ScadaPlusDbContext Create()
    {
        return new ScadaPlusDbContext(_options);
    }
}
