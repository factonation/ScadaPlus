using Microsoft.EntityFrameworkCore;
using ScadaPlus.Data.Contexts;
using ScadaPlus.Data.Models;

namespace ScadaPlus.Data.Repositories;

public class MachineRepository
{
    private readonly ScadaPlusDbContextFactory _factory;
    public MachineRepository(ScadaPlusDbContextFactory factory)
    {
        _factory = factory;
    }

    public async Task<List<Machine>> ReadMachinesAsync()
    {
        using (ScadaPlusDbContext context = _factory.Create())
        {
            return await context.Machines
                .Include(m => m.Jobs)
                .ToListAsync();
        }
    }
}
