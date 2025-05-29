using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

public class TransportContext : DbContext
{
    public DbSet<BusModel> Buses { get; set; } = null!;
    public DbSet<DriverModel> Drivers { get; set; } = null!;
    public DbSet<RouteModel> Routes { get; set; } = null!;

    public TransportContext(DbContextOptions<TransportContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One-to-One: Bus <-> Driver
        modelBuilder.Entity<BusModel>()
            .HasOne(b => b.Driver)
            .WithOne(d => d.Bus)
            .HasForeignKey<BusModel>(b => b.DriverId);

        // One-to-Many: Bus -> Routes
        modelBuilder.Entity<RouteModel>()
            .HasOne(r => r.Bus)
            .WithMany(b => b.Routes)
            .HasForeignKey(r => r.BusId);
    }
}