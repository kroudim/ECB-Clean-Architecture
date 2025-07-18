using ECB.API.Entities;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

public class ECBGatewayTests
  {
  private readonly Mock<IECBService> _ecbServiceMock;

  public ECBGatewayTests()
    {
    _ecbServiceMock = new Mock<IECBService>();
    }

  [Fact]
  public async Task GetExchangeRatesAsync_ShouldReturnRates()
    {
    // Arrange
    var expectedRates = new List<CurrencyRate>
        {
            new CurrencyRate("USD",1.0360m , DateTime.Now),
            new CurrencyRate("JPY",157.95m, DateTime.Now)
        };

    _ecbServiceMock.Setup(service => service.GetExchangeRatesAsync())
        .ReturnsAsync(expectedRates);

    // Act
    var rates = await _ecbServiceMock.Object.GetExchangeRatesAsync();

    // Assert
    Assert.NotNull(rates);
    Assert.Equal(2, rates.Count);
    Assert.Contains(rates, r => r.Currency == "USD" && r.Rate == 1.0360m);
    Assert.Contains(rates, r => r.Currency == "JPY" && r.Rate == 157.95m);
    }

  [Fact]
  public async Task GetExchangeRatesAsync_ShouldThrowExceptionOnHttpError()
    {
    // Arrange
    _ecbServiceMock.Setup(service => service.GetExchangeRatesAsync())
        .ThrowsAsync(new HttpRequestException());

    // Act & Assert
    await Assert.ThrowsAsync<HttpRequestException>(() => _ecbServiceMock.Object.GetExchangeRatesAsync());
    }
  }