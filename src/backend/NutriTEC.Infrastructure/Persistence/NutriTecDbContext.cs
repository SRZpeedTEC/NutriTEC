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

    public DbSet<Nutritionist> Nutritionists => Set<Nutritionist>();

    public DbSet<NutritionistClient> NutritionistClients => Set<NutritionistClient>();

    public DbSet<PlanMealTime> PlanMealTimes => Set<PlanMealTime>();

    public DbSet<Measure> Measures => Set<Measure>();

    public DbSet<NutritionPlan> NutritionPlans => Set<NutritionPlan>();

    public DbSet<PlanAssignment> PlanAssignments => Set<PlanAssignment>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<MealTime> MealTimes => Set<MealTime>();

    public DbSet<MealTimeProduct> MealTimeProducts => Set<MealTimeProduct>();

    public DbSet<DailyConsume> DailyConsumes => Set<DailyConsume>();

    public DbSet<DailyMealTime> DailyMealTimes => Set<DailyMealTime>();

    public DbSet<DailyConsumeDetail> DailyConsumeDetails => Set<DailyConsumeDetail>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<RecipeProduct> RecipeProducts => Set<RecipeProduct>();

    public DbSet<RecipeProductDetail> RecipeProductDetails => Set<RecipeProductDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations are kept in dedicated classes to keep the DbContext focused on persistence setup.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NutriTecDbContext).Assembly);
    }
}
