using System;
using System.Linq;
using System.Threading.Tasks;
using DoctorBooking.Data;
using DoctorBooking.Models;
using DoctorBooking.Models;
using DoctorBooking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly BookingService _bookingService;

        public AppointmentsController(ApplicationDbContext db, BookingService bookingService)
        {
            _db = db;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var appts = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderBy(a => a.Start)
                .ToListAsync();
            return View(appts);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _db.Patients.ToListAsync();
            ViewBag.Doctors = await _db.Doctors.ToListAsync();
            return View(new Appointment { DurationMinutes = 30, Start = DateTime.UtcNow.AddHours(1) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _db.Patients.ToListAsync();
                ViewBag.Doctors = await _db.Doctors.ToListAsync();
                return View(model);
            }

            var (success, error) = await _bookingService.CanBookAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                ViewBag.Patients = await _db.Patients.ToListAsync();
                ViewBag.Doctors = await _db.Doctors.ToListAsync();
                return View(model);
            }

            _db.Appointments.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Additional actions (Edit/Delete/Details) can be added similarly
    }
}
