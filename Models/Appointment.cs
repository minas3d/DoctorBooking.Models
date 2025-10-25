using System;

namespace DoctorBooking.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public DateTime Start { get; set; }
        public int DurationMinutes { get; set; } = 30;

        public DateTime End => Start.AddMinutes(DurationMinutes);
    }
}
