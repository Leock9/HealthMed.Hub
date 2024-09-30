using HealthMed.Hub.Domain.Patients;
using HealthMed.Hub.Domain.Patients.Gateways;
using Infrastructure.PostgreDb;
using Npgsql;
using System.Data;

namespace HealthMed.Hub.Infrastructure.PostgresDb.Gateways;

public class PatientGateway : IPatientGateway
{
    private readonly Context _context;

    public PatientGateway(Context context)
    {
        _context = context;
        EnsureTableExistsAsync().Wait();
    }

    public async Task<Patient> CreateAsync(Patient patient)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "INSERT INTO Patients (id, Name, Document, Email) VALUES (@id, @name, @document, @Email) RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("id", patient.Id);
        cmd.Parameters.AddWithValue("name", patient.Name);
        cmd.Parameters.AddWithValue("document", patient.Document);
        cmd.Parameters.AddWithValue("email", patient.Email);

        await cmd.ExecuteScalarAsync();
        conn.Close();

        return patient;
    }

    public async Task DeleteAsync(Guid id)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("DELETE FROM Patients WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        await cmd.ExecuteNonQueryAsync();
        conn.Close();
    }

    public async Task<IEnumerable<Patient>> GetAsync()
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, Name, Document, Email FROM Patients", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        var patients = new List<Patient>();
        while (await reader.ReadAsync())
        {
            patients.Add(new Patient(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetGuid(0)
            ));
        }

        conn.Close();
        return patients;
    }

    public async Task<Patient> GetByDocumentAsync(string document)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, Name, Document, Email FROM Patients WHERE Document = @document", conn);
        cmd.Parameters.AddWithValue("document", document);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var patient = new Patient(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetGuid(0)
            );
            conn.Close();
            return patient;
        }

        conn.Close();
        return null;
    }

    public async Task<Patient> GetByIdAsync(Guid id)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, Name, Document, Email FROM Patients WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var patient = new Patient(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetGuid(0)
            );
            conn.Close();
            return patient;
        }

        conn.Close();
        return null;
    }

    public async Task<Patient> UpdateAsync(Patient patient)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "UPDATE Patients SET Name = @name, Document = @document, Email = @Email WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", patient.Id);
        cmd.Parameters.AddWithValue("name", patient.Name);
        cmd.Parameters.AddWithValue("document", patient.Document);
        cmd.Parameters.AddWithValue("email", patient.Email);

        await cmd.ExecuteNonQueryAsync();
        conn.Close();

        return patient;
    }

    private async Task EnsureTableExistsAsync()
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "CREATE TABLE IF NOT EXISTS Patients (id uuid PRIMARY KEY, Name varchar(100), Document varchar(11), Email varchar(100))",
            conn
        );
        await cmd.ExecuteNonQueryAsync();
        conn.Close();
    }
}

