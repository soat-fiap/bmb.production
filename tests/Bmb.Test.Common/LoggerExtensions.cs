using Microsoft.Extensions.Logging;
using Moq;

namespace Bmb.Test.Common;

public static class LoggerExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, Action<ILogger<T>> verify, LogLevel logLevel,
        Times times)
    {
        loggerMock.Verify(
            x => x.Log(
                logLevel,
                0,
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}