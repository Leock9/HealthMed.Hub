using HealthMed.Hub.Domain.Doctors;
using HealthMed.Hub.Domain.Doctors.Gateways;
using Infrastructure.PostgreDb;
using Npgsql;
using System.Data;

namespace HealthMed.Hub.Infrastructure.PostgresDb.Gateways;

public class DoctorGateway : IDoctorGateway
{
    private readonly Context _context;

    public DoctorGateway(Context context)
    {
        _context = context;
        EnsureTableExistsAsync().Wait();
    }

    public async Task<Doctor> CreateAsync(Doctor doctor)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "INSERT INTO Doctors (id, Name, CRM, Document, Email) VALUES (@id, @name, @crm, @document, @Email) RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("id", doctor.Id.Value);
        cmd.Parameters.AddWithValue("name", doctor.Name);
        cmd.Parameters.AddWithValue("crm", doctor.Crm);
        cmd.Parameters.AddWithValue("document", doctor.Document);
        cmd.Parameters.AddWithValue("email", doctor.Email);

        await cmd.ExecuteScalarAsync();
        conn.Close();

        return doctor;
    }

    public async Task DeleteAsync(Guid id)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("DELETE FROM Doctors WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        await cmd.ExecuteNonQueryAsync();
        conn.Close();
    }

    public async Task<IEnumerable<Doctor>> GetAsync()
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, Name, CRM, Document, Email FROM Doctors", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        var doctors = new List<Doctor>();
        while (await reader.ReadAsync())
        {
            doctors.Add(new Doctor(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetGuid(0)
            ));
        }

        conn.Close();
        return doctors;
    }

    public async Task<Doctor> GetByCrmAsync(string crm)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, Name, CRM, Document, Email FROM Doctors WHERE CRM = @crm", conn);
        cmd.Parameters.AddWithValue("crm", crm);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var doctor = new Doctor(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetGuid(0)
            );
            conn.Close();
            return doctor;
        }

        conn.Close();
        return null;
    }

    public async Task<Doctor> GetByIdAsync(Guid id)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, Name, CRM, Document, Email FROM Doctors WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var doctor = new Doctor(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetGuid(0)
            );
            conn.Close();
            return doctor;
        }

        conn.Close();
        return null;
    }

    public async Task<Doctor> UpdateAsync(Doctor doctor)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "UPDATE Doctors SET Name = @name, CRM = @crm, Document = @document, Email = @Email WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", doctor.Id.Value);
        cmd.Parameters.AddWithValue("name", doctor.Name);
        cmd.Parameters.AddWithValue("crm", doctor.Crm);
        cmd.Parameters.AddWithValue("document", doctor.Document);
        cmd.Parameters.AddWithValue("email", doctor.Email);

        await cmd.ExecuteNonQueryAsync();
        conn.Close();

        return doctor;
    }

    private async Task EnsureTableExistsAsync()
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "CREATE TABLE IF NOT EXISTS Doctors (id uuid PRIMARY KEY, Name varchar(100), CRM varchar(100), Document varchar(11), Email varchar(100))",
            conn
        );
        await cmd.ExecuteNonQueryAsync();
        conn.Close();
    }
}
