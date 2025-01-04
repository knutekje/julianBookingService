using Microsoft.EntityFrameworkCore;
using BookingService.Models;

namespace BookingService.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .Property(b => b.CheckInDate)
            .HasColumnType("timestamp with time zone");

        modelBuilder.Entity<Booking>()
            .Property(b => b.CheckOutDate)
            .HasColumnType("timestamp with time zone");
    }

}