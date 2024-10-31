using System.Diagnostics.CodeAnalysis;

namespace Bmb.Production.Redis;

[ExcludeFromCodeCoverage]
public record RedisSettings
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 6379;
};
