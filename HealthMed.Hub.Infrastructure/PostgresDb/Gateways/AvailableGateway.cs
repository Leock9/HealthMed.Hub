using HealthMed.Hub.Domain.AvailableTimes;
using HealthMed.Hub.Domain.AvailableTimes.Gateways;
using Infrastructure.PostgreDb;
using Npgsql;
using NpgsqlTypes;

namespace HealthMed.Hub.Infrastructure.PostgresDb.Gateways;

public class AvailableGateway : IAvailableTimeGateway
{
    private readonly Context _context;

    public AvailableGateway(Context context)
    {
        _context = context;
        EnsureTableExistsAsync().Wait();
    }

    public async Task<AvaliableTime> CreateAsync(AvaliableTime avaliableTime)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "INSERT INTO AvailableTimes (id, DoctorId, StartTime, EndTime, DayOfWeek, IsOccupied) VALUES (@id, @doctorId, @startTime, @endTime, @dayOfWeek, @isOccupied) RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("id", avaliableTime.Id);
        cmd.Parameters.AddWithValue("doctorId", avaliableTime.DoctorId);
        cmd.Parameters.AddWithValue("startTime", avaliableTime.StartTime);
        cmd.Parameters.AddWithValue("endTime", avaliableTime.EndTime);
        cmd.Parameters.AddWithValue("dayOfWeek", (int)avaliableTime.DayOfWeek).NpgsqlDbType = NpgsqlDbType.Integer;
        cmd.Parameters.AddWithValue("isOccupied", avaliableTime.IsOccupied);

        await cmd.ExecuteScalarAsync();
        conn.Close();

        return avaliableTime;
    }
    public async Task DeleteAsync(Guid id)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("DELETE FROM AvailableTimes WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        await cmd.ExecuteNonQueryAsync();
        conn.Close();
    }
    public async Task<IEnumerable<AvaliableTime>> GetAsync()
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, DoctorId, StartTime, EndTime, DayOfWeek, IsOccupied FROM AvailableTimes", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        var availableTimes = new List<AvaliableTime>();
        while (await reader.ReadAsync())
        {
            var startTime = reader.GetTimeSpan(2);
            var endTime = reader.GetTimeSpan(3);

            var timeOnlyStartTime = TimeOnly.FromTimeSpan(startTime);
            var timeOnlyEndTime = TimeOnly.FromTimeSpan(endTime);

            availableTimes.Add(new AvaliableTime(
                timeOnlyStartTime,
                timeOnlyEndTime,
                reader.GetGuid(1),
                (DayOfWeek)reader.GetInt32(4))
            {
                Id = reader.GetGuid(0),
                IsOccupied = reader.GetBoolean(5)
            });
        }

        conn.Close();
        return availableTimes;
    }

    public async Task<IEnumerable<AvaliableTime>> GetByDoctorIdAsync(Guid doctorId)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, DoctorId, StartTime, EndTime, DayOfWeek, IsOccupied FROM AvailableTimes WHERE DoctorId = @doctorId", conn);
        cmd.Parameters.AddWithValue("doctorId", doctorId);

        using var reader = await cmd.ExecuteReaderAsync();

        var availableTimes = new List<AvaliableTime>();
        while (await reader.ReadAsync())
        {
            var startTime = reader.GetTimeSpan(2);
            var endTime = reader.GetTimeSpan(3);

            var timeOnlyStartTime = TimeOnly.FromTimeSpan(startTime);
            var timeOnlyEndTime = TimeOnly.FromTimeSpan(endTime);

            availableTimes.Add(new AvaliableTime
            (
                timeOnlyStartTime,
                timeOnlyEndTime,
                reader.GetGuid(1),
                (DayOfWeek)reader.GetInt32(4)
            )
            {
                Id = reader.GetGuid(0),
                IsOccupied = reader.GetBoolean(5)
            });
        }

        conn.Close();
        return availableTimes;
    }

    public async Task<AvaliableTime> GetByIdAsync(Guid id)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, DoctorId, StartTime, EndTime, DayOfWeek, IsOccupied FROM AvailableTimes WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var startTime = reader.GetTimeSpan(2);
            var endTime = reader.GetTimeSpan(3);

            var timeOnlyStartTime = TimeOnly.FromTimeSpan(startTime);
            var timeOnlyEndTime = TimeOnly.FromTimeSpan(endTime);

            var availableTime = new AvaliableTime(
                timeOnlyStartTime,
                timeOnlyEndTime,
                reader.GetGuid(1),
                (DayOfWeek)reader.GetInt32(4))
            {
                Id = reader.GetGuid(0),
                IsOccupied = reader.GetBoolean(5)
            };
            conn.Close();
            return availableTime;
        }

        conn.Close();
        return null;
    }

    public async Task<AvaliableTime> UpdateAsync(AvaliableTime avaliableTime)
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "UPDATE AvailableTimes SET DoctorId = @doctorId, StartTime = @startTime, EndTime = @endTime, DayOfWeek = @dayOfWeek, IsOccupied = @isOccupied WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", avaliableTime.Id);
        cmd.Parameters.AddWithValue("doctorId", avaliableTime.DoctorId);
        cmd.Parameters.AddWithValue("startTime", avaliableTime.StartTime);
        cmd.Parameters.AddWithValue("endTime", avaliableTime.EndTime);
        cmd.Parameters.AddWithValue("dayOfWeek", (int)avaliableTime.DayOfWeek).NpgsqlDbType = NpgsqlDbType.Integer;
        cmd.Parameters.AddWithValue("isOccupied", avaliableTime.IsOccupied);

        await cmd.ExecuteNonQueryAsync();
        conn.Close();

        return avaliableTime;
    }

    private async Task EnsureTableExistsAsync()
    {
        var conn = _context.GetConnection();
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand
        (
            "CREATE TABLE IF NOT EXISTS AvailableTimes (id uuid PRIMARY KEY, DoctorId uuid, StartTime time, EndTime time, DayOfWeek int, IsOccupied boolean)",
            conn
        );
        await cmd.ExecuteNonQueryAsync();
        conn.Close();
    }
}
