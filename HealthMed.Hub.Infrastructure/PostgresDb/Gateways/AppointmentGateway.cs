using HealthMed.Hub.Domain.Appointments;
using HealthMed.Hub.Domain.Appointments.Gateways;
using Infrastructure.PostgreDb;
using Npgsql;
using System.Data;

namespace HealthMed.Hub.Infrastructure.PostgresDb.Gateways;

public class AppointmentGateway : IAppointmentsGateway
{
    private readonly Context _context;

    public AppointmentGateway(Context context)
    {
        _context = context;
        EnsureTableExistsAsync().Wait();
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "INSERT INTO Appointments (id, DoctorId, PatientId, AvailableTimeId, Date) VALUES (@id, @doctorId, @patientId, @availableTimeId, @date) RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("id", appointment.Id);
        cmd.Parameters.AddWithValue("doctorId", appointment.DoctorId);
        cmd.Parameters.AddWithValue("patientId", appointment.PatientId);
        cmd.Parameters.AddWithValue("availableTimeId", appointment.AvailableTimeId);
        cmd.Parameters.AddWithValue("date", appointment.Date);

        await cmd.ExecuteScalarAsync();
        conn.Close();

        return appointment;
    }

    public async Task DeleteAsync(Guid id)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("DELETE FROM Appointments WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        await cmd.ExecuteNonQueryAsync();
        conn.Close();
    }

    public async Task<IEnumerable<Appointment>> GetAsync()
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, DoctorId, PatientId, AvailableTimeId, Date FROM Appointments", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        var appointments = new List<Appointment>();
        while (await reader.ReadAsync())
        {
            appointments.Add(new Appointment(
                reader.GetGuid(1),
                reader.GetGuid(2),
                reader.GetGuid(3),
                reader.GetDateTime(4))
            {
                Id = reader.GetGuid(0)
            });
        }

        conn.Close();
        return appointments;
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, DoctorId, PatientId, AvailableTimeId, Date FROM Appointments WHERE DoctorId = @doctorId", conn);
        cmd.Parameters.AddWithValue("doctorId", doctorId);

        using var reader = await cmd.ExecuteReaderAsync();

        var appointments = new List<Appointment>();
        while (await reader.ReadAsync())
        {
            appointments.Add(new Appointment(
                reader.GetGuid(1),
                reader.GetGuid(2),
                reader.GetGuid(3),
                reader.GetDateTime(4))
            {
                Id = reader.GetGuid(0)
            });
        }

        conn.Close();
        return appointments;
    }

    public async Task<Appointment> GetByIdAsync(Guid id)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, DoctorId, PatientId, AvailableTimeId, Date FROM Appointments WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var appointment = new Appointment(
                reader.GetGuid(1),
                reader.GetGuid(2),
                reader.GetGuid(3),
                reader.GetDateTime(4))
            {
                Id = reader.GetGuid(0)
            };
            conn.Close();
            return appointment;
        }

        conn.Close();
        return null;
    }

    public async Task<IEnumerable<Appointment>> GetByPatientAsync(Guid patientId)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, DoctorId, PatientId, AvailableTimeId, Date FROM Appointments WHERE PatientId = @patientId", conn);
        cmd.Parameters.AddWithValue("patientId", patientId);

        using var reader = await cmd.ExecuteReaderAsync();

        var appointments = new List<Appointment>();
        while (await reader.ReadAsync())
        {
            appointments.Add(new Appointment(
                reader.GetGuid(1),
                reader.GetGuid(2),
                reader.GetGuid(3),
                reader.GetDateTime(4))
            {
                Id = reader.GetGuid(0)
            });
        }

        conn.Close();
        return appointments;
    }

    public async Task<Appointment> UpdateAsync(Appointment appointment)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "UPDATE Appointments SET DoctorId = @doctorId, PatientId = @patientId, AvailableTimeId = @availableTimeId, Date = @date WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", appointment.Id);
        cmd.Parameters.AddWithValue("doctorId", appointment.DoctorId);
        cmd.Parameters.AddWithValue("patientId", appointment.PatientId);
        cmd.Parameters.AddWithValue("availableTimeId", appointment.AvailableTimeId);
        cmd.Parameters.AddWithValue("date", appointment.Date);

        await cmd.ExecuteNonQueryAsync();
        conn.Close();

        return appointment;
    }

    private async Task EnsureTableExistsAsync()
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "CREATE TABLE IF NOT EXISTS Appointments (id uuid PRIMARY KEY, DoctorId uuid, PatientId uuid, AvailableTimeId uuid, Date timestamp)",
            conn
        );
        await cmd.ExecuteNonQueryAsync();
        conn.Close();
    }
}
