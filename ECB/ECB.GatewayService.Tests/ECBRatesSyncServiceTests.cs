using ECB.Application;
using ECB.Infrastructure.Application;
using ECB.Infrastructure.Gateway.Service;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class ECBRatesSyncServiceTests
    {
    private readonly Mock<ILogger<EcbRatesSyncService>> _loggerMock;
    private readonly Mock<IEcbRatesGateway> _ecbServiceMock;
    private readonly Mock<IDatabaseService> _databaseServiceMock;
    private readonly Mock<ICurrencyRatesCache> _cacheMock;
    private readonly Mock<IOptions<EcbRatesSyncOptions>> _optionsMock;
    private readonly EcbRatesSyncService _ecbJob;
    private readonly double _interval = 1.0; // 1 minute

    public ECBRatesSyncServiceTests()
        {
        _loggerMock = new Mock<ILogger<EcbRatesSyncService>>();
        _ecbServiceMock = new Mock<IEcbRatesGateway>();
        _databaseServiceMock = new Mock<IDatabaseService>();
        _cacheMock = new Mock<ICurrencyRatesCache>();
        _optionsMock = new Mock<IOptions<EcbRatesSyncOptions>>();
        _optionsMock.Setup(x => x.Value).Returns(new EcbRatesSyncOptions
            {
            ECBRatesInterval = _interval
            });

        _ecbJob = new EcbRatesSyncService(_loggerMock.Object, _ecbServiceMock.Object, _databaseServiceMock.Object, _cacheMock.Object, _optionsMock.Object);
        }

    [Fact]
    public async Task UpdateDatabaseAsync_ShouldUpdateDatabaseWithRates()
        {
        // Arrange
        var rates = new List<CurrencyRateDto>
        {
            new CurrencyRateDto("USD", 1.0360m, DateTime.Now),
            new CurrencyRateDto("JPY", 157.95m, DateTime.Now)
        };

        _databaseServiceMock.Setup(db => db.OpenConnectionAsync()).Returns(Task.CompletedTask);
        _databaseServiceMock.Setup(db => db.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _databaseServiceMock.Setup(db => db.CommitTransactionAsync()).Returns(Task.CompletedTask);
        _databaseServiceMock.Setup(db => db.RollbackTransactionAsync()).Returns(Task.CompletedTask);
        _databaseServiceMock.Setup(db => db.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
            .ReturnsAsync(1);

        // Act
        await _ecbJob.UpdateDatabaseAsync(rates);

        // Assert
        _databaseServiceMock.Verify(db => db.OpenConnectionAsync(), Times.Once);
        _databaseServiceMock.Verify(db => db.BeginTransactionAsync(), Times.Once);
        _databaseServiceMock.Verify(db => db.CommitTransactionAsync(), Times.Once);
        _databaseServiceMock.Verify(db => db.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Exactly(rates.Count));
        }
    }
