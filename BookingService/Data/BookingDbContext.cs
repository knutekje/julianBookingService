using Microsoft.EntityFrameworkCore;
using BookingService.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookingService.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookedRoom> BookedRooms { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .Property(b => b.CheckInDate)
            .HasColumnType("timestamp with time zone");

        modelBuilder.Entity<Booking>()
            .Property(b => b.CheckOutDate)
            .HasColumnType("timestamp with time zone");
        
        
        modelBuilder.Entity<BookedRoom>()
            .HasKey(br => new { br.RoomId, br.DateBooked });
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion(dateTimeConverter);
            }
        }
 
    }

}