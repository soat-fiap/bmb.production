namespace Bmb.Production.Redis.Repository;

public record RedisSettings
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 6379;
};
