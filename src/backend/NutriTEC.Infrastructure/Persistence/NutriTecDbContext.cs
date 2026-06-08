using Microsoft.EntityFrameworkCore;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Persistence;

public class NutriTecDbContext : DbContext
{
    public NutriTecDbContext(DbContextOptions<NutriTecDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<Measure> Measures => Set<Measure>();

    public DbSet<NutritionPlan> NutritionPlans => Set<NutritionPlan>();

    public DbSet<PlanAssignment> PlanAssignments => Set<PlanAssignment>();

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations are kept in dedicated classes to keep the DbContext focused on persistence setup.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NutriTecDbContext).Assembly);
    }
}
