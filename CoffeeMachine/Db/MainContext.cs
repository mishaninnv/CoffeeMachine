using CoffeeMachine.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachine.Db;

public class MainContext : DbContext
{
    public DbSet<CoffeeModel> Coffee { get; set; } = null!;
    public DbSet<PaymentTypeModel> PaymentTypes { get; set; } = null!;
    public DbSet<RequestModel> Requests { get; set; } = null!;
    public DbSet<ResourceModel> Resources { get; set; } = null!;
    public DbSet<UnitModel> Units { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=coffeemachine.db");
    }
}
