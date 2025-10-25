using DoctorBooking.Models;
using DoctorBooking.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DoctorBooking.Data
{
    public static class BookingRules
    {
        // Minimum gap between patient's appointments in minutes
        public const int MinimumPatientGapMinutes = 30;
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public ApplicationDbContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;" +
                "Initial Catalog=DoctorBooking;" +
                "Integrated Security=True;" +
                "Connect Timeout=30;Encrypt=True;" +
                "Trust Server Certificate=True;" +
                "Application Intent=ReadWrite;" +
                "Multi Subnet Failover=False");

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Doctors
            builder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FullName = "Dr. Ahmed Ali", Specialization = "General" },
                new Doctor { Id = 2, FullName = "Dr. Salma Hassan", Specialization = "Pediatrics" },
                new Doctor { Id = 3, FullName = "Dr. Omar Nabil", Specialization = "Dermatology" }
            );

            // Seed Patients
            builder.Entity<Patient>().HasData(
                new Patient { Id = 1, FullName = "Mona Mohamed", Email = "mona@example.com" },
                new Patient { Id = 2, FullName = "Youssef Adel", Email = "youssef@example.com" }
            );

            // Seed Appointments (example - times are UTC kind)
            builder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, PatientId = 1, DoctorId = 1, Start = new DateTime(2025, 11, 1, 10, 0, 0), DurationMinutes = 30 },
                new Appointment { Id = 2, PatientId = 2, DoctorId = 2, Start = new DateTime(2025, 11, 1, 10, 30, 0), DurationMinutes = 30 }
            );
        }
    }
}
