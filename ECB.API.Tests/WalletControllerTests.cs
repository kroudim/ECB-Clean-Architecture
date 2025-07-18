using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ECB.API.Controllers;
using Assert = Xunit.Assert;
using ECB.Application;
using ECB.Domain;

namespace ECB.API.Tests
    {
    public class WalletControllerTests
        {
        private readonly Mock<IWalletService> _mockWalletService;
        private readonly WalletController _controller;

        public WalletControllerTests()
            {
            _mockWalletService = new Mock<IWalletService>();
            _controller = new WalletController(_mockWalletService.Object);
            }

        // Test for Creating a Wallet
        [Fact]
        public async Task CreateWallet_Returns_CreatedAtAction()
            {
            // Arrange
            var newWallet = new Wallet(1, "USD", 100);
            var newWalletdto = new CreateWalletDto(100, "USD");
            _mockWalletService.Setup(s => s.CreateWalletAsync(It.IsAny<Wallet>()))
                              .ReturnsAsync(newWallet);

            // Act
            var result = await _controller.CreateWallet(newWalletdto);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdWallet = Assert.IsType<Wallet>(actionResult.Value);
            Assert.Equal(1, createdWallet.Id);
            Assert.Equal(100, createdWallet.Balance);
            }

        // Test for Retrieving Wallet Balance (Success)
        [Fact]
        public async Task GetWalletBalance_Returns_Ok_With_Balance()
            {
            // Arrange
            long walletId = 1;
            string currency = "USD";
            decimal expectedBalance = 100;

            _mockWalletService.Setup(s => s.GetBalanceAsync(walletId, currency))
                              .ReturnsAsync(expectedBalance);

            // Act
            var result = await _controller.GetWalletBalance(walletId, currency);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var balanceResponse = Assert.IsType<Decimal>(okResult.Value);


            Assert.Equal(expectedBalance, balanceResponse);

            }

        // Test for Retrieving Non-Existent Wallet (Should Return NotFound)
        [Fact]
        public async Task GetWalletBalance_WalletNotFound_Returns_NotFound()
            {
            // Arrange
            _mockWalletService.Setup(s => s.GetBalanceAsync(It.IsAny<long>(), It.IsAny<string>()))
                              .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetWalletBalance(999, "USD");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            }

        // Test for Adjusting Balance (AddFundsStrategy)
        [Fact]
        public async Task AdjustBalance_AddFundsStrategy_Returns_Ok()
            {
            // Arrange
            long walletId = 1;
            decimal amount = 50;
            string currency = "USD";
            string strategy = "AddFundsStrategy";

            _mockWalletService.Setup(s => s.AdjustBalanceAsync(walletId, amount, currency, It.IsAny<IBalanceStrategy>()))
                              .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AdjustBalance(walletId, amount, currency, strategy);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Balance adjusted successfully", okResult.Value);
            }

        // Test for Adjusting Balance with Invalid Strategy
        [Fact]
        public async Task AdjustBalance_InvalidStrategy_Returns_BadReques()
            {
            // Arrange
            long walletId = 1;
            decimal amount = 50;
            string currency = "USD";
            string strategy = "InvalidStrategy";

            // Act
            var result = await _controller.AdjustBalance(walletId, amount, currency, strategy);
            var okResult = Assert.IsType<ObjectResult>(result);

            // Assert
            Assert.Equal(500, okResult.StatusCode);
            }

        // Test for Adjusting Balance with Insufficient Funds (SubtractFundsStrategy)
        [Fact]
        public async Task AdjustBalance_InsufficientFunds_Returns_BadRequest()
            {
            // Arrange
            long walletId = 1;
            decimal amount = 500;
            string currency = "USD";
            string strategy = "SubtractFundsStrategy";

            _mockWalletService.Setup(s => s.AdjustBalanceAsync(walletId, amount, currency, It.IsAny<IBalanceStrategy>()))
                              .ThrowsAsync(new InvalidOperationException("Insufficient funds"));

            // Act
            var result = await _controller.AdjustBalance(walletId, amount, currency, strategy);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Insufficient funds", badRequestResult.Value);
            }
        }
    }