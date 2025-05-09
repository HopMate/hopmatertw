using Microsoft.EntityFrameworkCore;
using System;
using hopmate.Server.Models.Entities;

namespace hopmate.Server.Data
{
    public static class ModelBuilderExtensions
    {
        public static void SeedStaticData(this ModelBuilder modelBuilder)
        {
            // RequestStatus
            modelBuilder.Entity<RequestStatus>().HasData(
                new RequestStatus { Id = 1, Status = "Pending" },
                new RequestStatus { Id = 2, Status = "Accepted" },
                new RequestStatus { Id = 3, Status = "Rejected" },
                new RequestStatus { Id = 4, Status = "WaitingList" },
                new RequestStatus { Id = 5, Status = "Cancelled" }
            );

            // Color
            modelBuilder.Entity<Color>().HasData(
                new Color { Id = 1, Name = "Black" },
                new Color { Id = 2, Name = "White" },
                new Color { Id = 3, Name = "Red" },
                new Color { Id = 4, Name = "Blue" },
                new Color { Id = 5, Name = "Gray" },
                new Color { Id = 6, Name = "Silver" },
                new Color { Id = 7, Name = "Green" },
                new Color { Id = 8, Name = "Yellow" },
                new Color { Id = 9, Name = "Orange" },
                new Color { Id = 10, Name = "Brown" }
            );

            // TripStatus
            modelBuilder.Entity<TripStatus>().HasData(
                new TripStatus { Id = 1, Status = "Planned" },
                new TripStatus { Id = 2, Status = "In Progress" },
                new TripStatus { Id = 3, Status = "Completed" },
                new TripStatus { Id = 4, Status = "Cancelled" }
            );
        }
    }
}
