using Microsoft.EntityFrameworkCore;
using ScadaPlus.Data.Contexts;
using ScadaPlus.Data.Models;

namespace ScadaPlus.Data.Repositories;

public class JobRepository
{
    private readonly ScadaPlusDbContextFactory _factory;
    public JobRepository(ScadaPlusDbContextFactory factory)
    {
        _factory = factory;
    }

    public async Task<List<Job>> ReadJobsAsync()
    {
        using (ScadaPlusDbContext context = _factory.Create())
        {
            return await context.Jobs
                .Include(j => j.Machine)
                .OrderByDescending(j => j.JobId)
                .ToListAsync();
        }
    }

    public async Task CreateJobAsync(Job job)
    {
        using (ScadaPlusDbContext context = _factory.Create())
        {
            context.Jobs.Add(job);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateJobAsync(Job job)
    {
        using (ScadaPlusDbContext context = _factory.Create())
        {
            var existingJob = await context.Jobs.FirstOrDefaultAsync(j => j.JobId == job.JobId);

            if (existingJob is null) return;

            existingJob.IdealCycleTime = job.IdealCycleTime;
            existingJob.OkCount = job.OkCount;
            existingJob.NgCount = job.NgCount;
            existingJob.Availability = job.Availability;
            existingJob.Performance = job.Performance;
            existingJob.Quality = job.Quality;
            existingJob.OEE = job.OEE;

            await context.SaveChangesAsync();
        }
    }
}
