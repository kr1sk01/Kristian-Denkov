using Microsoft.EntityFrameworkCore;
using TermisWorkerService;

public class CsvContext : DbContext
{
    public DbSet<CsvData> CsvData { get; set; } = default!;
    public DbSet<Master> Masters { get; set; } = default!;

    public CsvContext(DbContextOptions<CsvContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<CsvData>().Property(x => x.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Master>().Property(x => x.Id).ValueGeneratedOnAdd();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Integrated Security=True;Database=Termis");
        }
    }
}
