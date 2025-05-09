using hopmate.Server.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace hopmate.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet for every model
        public DbSet<Penalty> Penalties { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<VoucherStatus> VoucherStatuses { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<PassengerTrip> PassengerTrips { get; set; }
        public DbSet<RequestStatus> RequestStatuses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripLocation> TripLocations { get; set; }
        public DbSet<TripStatus> TripStatuses { get; set; }
        public DbSet<UserVoucher> UserVouchers { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.SeedStaticData();

            var camelCaseToSnakeCase = new Regex("([a-z0-9])([A-Z])");

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName()?.ToLower());

                foreach (var property in entity.GetProperties())
                {
                    var columnName = camelCaseToSnakeCase.Replace(property.Name, "$1_$2").ToLower();
                    property.SetColumnName(columnName);
                }
            }

            // Configure ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("user");

                entity.Property(u => u.DateOfBirth)
                    .HasConversion(
                        new ValueConverter<DateOnly, DateTime>(
                            d => d.ToDateTime(TimeOnly.MinValue),
                            d => DateOnly.FromDateTime(d)));
            });

            builder.Entity<IdentityRole<Guid>>().ToTable("role");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");

            builder.Entity<Driver>().ToTable("driver");
            builder.Entity<Color>().ToTable("color");
            builder.Entity<Image>().ToTable("image");
            builder.Entity<Location>().ToTable("location");
            builder.Entity<Passenger>().ToTable("passenger");
            builder.Entity<PassengerTrip>().ToTable("passenger_trip");
            builder.Entity<Penalty>().ToTable("penalty");
            builder.Entity<RequestStatus>().ToTable("request_status");
            builder.Entity<Review>().ToTable("review");
            builder.Entity<Reward>().ToTable("reward");
            builder.Entity<Sponsor>().ToTable("sponsor");
            builder.Entity<Trip>().ToTable("trip");
            builder.Entity<TripLocation>().ToTable("trip_location");
            builder.Entity<TripStatus>().ToTable("trip_status");
            builder.Entity<UserVoucher>().ToTable("user_voucher");
            builder.Entity<Vehicle>().ToTable("vehicle");
            builder.Entity<Voucher>().ToTable("voucher");
            builder.Entity<VoucherStatus>().ToTable("voucher_status");


            builder.Entity<Color>()
                .HasMany(c => c.Vehicles)
                .WithOne(v => v.Color)
                .HasForeignKey(v => v.IdColor)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Penalties)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.IdUser);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.MemberVouchers)
                .WithOne(uv => uv.User)
                .HasForeignKey(uv => uv.IdUser);

            builder.Entity<Voucher>()
                .HasMany(v => v.MemberVouchers)
                .WithOne(uv => uv.Voucher)
                .HasForeignKey(uv => uv.IdVoucher);

            builder.Entity<Voucher>()
                .HasOne(v => v.Image)
                .WithMany(i => i.Vouchers)
                .HasForeignKey(v => v.IdImage);

            builder.Entity<Voucher>()
                .HasOne(v => v.Sponsor)
                .WithMany(s => s.Vouchers)
                .HasForeignKey(v => v.IdSponsor);

            builder.Entity<Voucher>()
                .HasOne(v => v.VoucherStatus)
                .WithMany(vs => vs.Vouchers)
                .HasForeignKey(v => v.IdVoucherStatus);

            builder.Entity<Driver>()
                .HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<Driver>(d => d.IdUser)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Driver>()
                .HasMany(d => d.Reviews)
                .WithOne(r => r.Driver)
                .HasForeignKey(r => r.IdDriver)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Driver>()
                .HasMany(d => d.Rewards)
                .WithOne(r => r.Driver)
                .HasForeignKey(r => r.IdDriver);

            builder.Entity<Driver>()
                .HasMany(d => d.Vehicles)
                .WithOne(v => v.Driver)
                .HasForeignKey(v => v.IdDriver)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Driver>()
                .HasMany(d => d.Trips)
                .WithOne(t => t.Driver)
                .HasForeignKey(t => t.IdDriver)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Vehicle>()
                .HasOne(v => v.Color)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.IdColor)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vehicle>()
                .HasOne(v => v.Driver)
                .WithMany(d => d.Vehicles)
                .HasForeignKey(v => v.IdDriver);

            builder.Entity<Vehicle>()
                .HasMany(v => v.Trips)
                .WithOne(t => t.Vehicle)
                .HasForeignKey(t => t.IdVehicle)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Location>()
                .HasMany(l => l.TripLocations)
                .WithOne(tl => tl.Location)
                .HasForeignKey(tl => tl.IdLocation);

            builder.Entity<Passenger>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.IdUser)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Passenger>()
                .HasMany(p => p.Reviews)
                .WithOne(r => r.Passenger)
                .HasForeignKey(r => r.IdPassenger)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Passenger>()
                .HasMany(p => p.PassengerTrips)
                .WithOne(pt => pt.Passenger)
                .HasForeignKey(pt => pt.IdPassenger);

            builder.Entity<PassengerTrip>()
                .HasOne(pt => pt.Location)
                .WithMany()
                .HasForeignKey(pt => pt.IdLocation)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PassengerTrip>()
                .HasOne(pt => pt.RequestStatus)
                .WithMany(rs => rs.PassengerTrips)
                .HasForeignKey(pt => pt.IdRequestStatus)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Penalty>()
                .HasOne(p => p.User)
                .WithMany(u => u.Penalties)
                .HasForeignKey(p => p.IdUser);

            builder.Entity<Review>()
                .HasOne(r => r.Driver)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.IdDriver)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Review>()
                .HasOne(r => r.Passenger)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.IdPassenger)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Reward>()
                .HasOne(r => r.Driver)
                .WithMany(d => d.Rewards)
                .HasForeignKey(r => r.IdDriver);

            builder.Entity<Trip>()
                .HasOne(t => t.Driver)
                .WithMany(d => d.Trips)
                .HasForeignKey(t => t.IdDriver)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Trip>()
                .HasOne(t => t.Vehicle)
                .WithMany(v => v.Trips)
                .HasForeignKey(t => t.IdVehicle)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Trip>()
                .HasOne(t => t.TripStatus)
                .WithMany(ts => ts.Trips)
                .HasForeignKey(t => t.IdStatusTrip)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TripLocation>()
                .HasOne(tl => tl.Trip)
                .WithMany(t => t.TripLocations)
                .HasForeignKey(tl => tl.IdTrip);

            builder.Entity<TripLocation>()
                .HasOne(tl => tl.Location)
                .WithMany(l => l.TripLocations)
                .HasForeignKey(tl => tl.IdLocation);

            builder.Entity<TripStatus>()
                .HasMany(ts => ts.Trips)
                .WithOne(t => t.TripStatus)
                .HasForeignKey(t => t.IdStatusTrip)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserVoucher>()
                .HasOne(uv => uv.User)
                .WithMany(u => u.MemberVouchers)
                .HasForeignKey(uv => uv.IdUser);

            builder.Entity<UserVoucher>()
                .HasOne(uv => uv.Voucher)
                .WithMany(v => v.MemberVouchers)
                .HasForeignKey(uv => uv.IdVoucher)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
