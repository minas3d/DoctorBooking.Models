using System;
using System.Linq;
using System.Threading.Tasks;
using DoctorBooking.Data;
using DoctorBooking.Models;
using DoctorBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.Services
{
    public class BookingService
    {
        private readonly ApplicationDbContext _db;

        public BookingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<(bool Success, string Error)> CanBookAsync(Appointment appointment)
        {
            var newStart = appointment.Start;
            var newEnd = appointment.End;

            // 1) Check if same doctor has overlapping appointments
            var doctorOverlaps = await _db.Appointments
                .Where(a => a.DoctorId == appointment.DoctorId && a.Id != appointment.Id)
                .Where(a => a.Start < newEnd && a.Start.AddMinutes(a.DurationMinutes) > newStart)
                .AnyAsync();

            if (doctorOverlaps)
                return (false, "The selected doctor has another appointment overlapping that time.");

            // 2) Check patient's existing appointments (minimum 30-minute gap)
            var patientAppointments = await _db.Appointments
                .Where(a => a.PatientId == appointment.PatientId && a.Id != appointment.Id)
                .ToListAsync();

            foreach (var existing in patientAppointments)
            {
                var existingEnd = existing.Start.AddMinutes(existing.DurationMinutes);

                // If overlap
                if (newStart < existingEnd && newEnd > existing.Start)
                    return (false, $"You already have an overlapping appointment (existing at {existing.Start}).");

                // If gap less than 30 minutes
                var gapBefore = (newStart - existingEnd).TotalMinutes;
                var gapAfter = (existing.Start - newEnd).TotalMinutes;

                if (gapBefore < 30 && gapBefore > -30)
                    return (false, "You must leave at least 30 minutes between appointments.");
                if (gapAfter < 30 && gapAfter > -30)
                    return (false, "You must leave at least 30 minutes between appointments.");
            }

            return (true, null);
        }
    }
}
