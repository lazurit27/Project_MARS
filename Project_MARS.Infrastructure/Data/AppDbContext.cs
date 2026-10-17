using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<AutoService> AutoServices { get; set; }
        public DbSet<RepairBox> RepairBoxes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<TimeFlag> TimeFlags { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<RepairBoxAssignment> RepairBoxAssignments { get; set; }
        public DbSet<AutoServiceBoss> AutoServiceBosses { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Car>()
                .HasIndex(c => c.Vin)
                .IsUnique();


            modelBuilder.Entity<AutoService>()
                .HasOne(s => s.AutoServiceBoss)
                .WithMany(b => b.AutoServices)
                .HasForeignKey(s => s.AutoServiceBossId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RepairBox>()
                .HasOne(b => b.AutoService)
                .WithMany(s => s.RepairBoxes)
                .HasForeignKey(b => b.AutoServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rate>()
                .HasOne(b => b.AutoService)
                .WithMany(s => s.Rates)
                .HasForeignKey(b => b.AutoServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RepairBox>()
                .HasOne(b => b.ServiceCategory)
                .WithMany()
                .HasForeignKey(b => b.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkSchedule>()
                .HasOne(ws => ws.AutoService)
                .WithMany(s => s.WorkSchedules)
                .HasForeignKey(ws => ws.AutoServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }


}