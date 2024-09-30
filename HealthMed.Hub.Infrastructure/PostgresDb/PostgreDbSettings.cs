namespace Infrastructure.PostgreDb;

public interface IPostgreDbSettings
{
    string PostgresConnection { get; }
}

public record PostgreDbSettings : IPostgreDbSettings
{
    public string PostgresConnection { get; init; }

    public PostgreDbSettings(string postgresConnection) => PostgresConnection = postgresConnection;
}
