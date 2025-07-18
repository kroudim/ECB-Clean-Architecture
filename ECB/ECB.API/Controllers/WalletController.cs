using ECB.Application;
using ECB.Domain;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace ECB.API.Controllers
    {
    /// <summary>
    /// Controller for wallet operations such as creation, balance retrieval, and balance adjustments.
    /// </summary>
    [ApiController]
    [Route("api/wallets")]
    public class WalletController : ControllerBase
        {
        private readonly IWalletService _walletService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletController"/> class.
        /// </summary>
        /// <param name="walletService">The wallet service used to perform wallet operations.</param>
        public WalletController(IWalletService walletService)
            {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            }

        /// <summary>
        /// Creates a new wallet.
        /// </summary>
        /// <param name="walletdto">The wallet DTO containing currency and initial balance.</param>
        /// <returns>A <see cref="CreatedAtActionResult"/> with the created wallet.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletDto walletdto)
            {
            // Construct a new wallet entity; Id will be set by the database
            var wallet = new Wallet
                {
                Currency = walletdto.Currency,
                Balance = walletdto.Balance
                };
            var createdWallet = await _walletService.CreateWalletAsync(wallet);

            // Return 201 Created with a link to the balance endpoint
            return CreatedAtAction(
                nameof(GetWalletBalance),
                new { walletId = createdWallet.Id },
                createdWallet
            );
            }

        /// <summary>
        /// Gets the balance of a wallet.
        /// </summary>
        /// <param name="walletId">The ID of the wallet.</param>
        /// <param name="currency">The currency to get the balance in (optional).</param>
        /// <returns>
        /// An <see cref="OkObjectResult"/> with the wallet balance, or <see cref="NotFoundResult"/> if not found.
        /// </returns>
        [HttpGet("{walletId}")]
        public async Task<IActionResult> GetWalletBalance(
            long walletId,
            [FromQuery] string? currency = "")
            {
            try
                {
                // Get the balance from the wallet service, and return as Ok
                var balance = await _walletService.GetBalanceAsync(walletId, currency);
                var wallet = new WalletDto(walletId, balance, currency);

                return Ok(wallet.Balance);
                }
            catch (Exception)
                {
                // Return 404 if the wallet is not found or another error occurs
                return NotFound("Wallet not found");
                }
            }

        /// <summary>
        /// Adjusts the balance of a wallet using the specified strategy.
        /// </summary>
        /// <param name="walletId">The ID of the wallet.</param>
        /// <param name="amount">The amount to adjust the balance by. Must be positive.</param>
        /// <param name="currency">The currency of the amount.</param>
        /// <param name="strategy">
        /// The strategy for adjusting the balance. Options:
        /// <list type="bullet">
        /// <item>"addfundsstrategy"</item>
        /// <item>"subtractfundsstrategy"</item>
        /// <item>"forcesubtractfundsstrategy"</item>
        /// </list>
        /// </param>
        /// <returns>
        /// <see cref="OkResult"/> if successful,
        /// <see cref="BadRequestResult"/> if insufficient funds or input invalid,
        /// <see cref="StatusCodeResult"/> 500 if another error occurs.
        /// </returns>
        [HttpPost("{walletId}/adjustbalance")]
        public async Task<IActionResult> AdjustBalance(
            long walletId,
            [FromQuery] decimal amount,
            [FromQuery] string currency,
            [FromQuery] string strategy
        )
            {
            try
                {
                // Amount must be positive
                if (amount <= 0) return BadRequest("Amount must be positive");

                // Choose strategy based on input string
                IBalanceStrategy balanceStrategy = strategy.ToLower() switch
                    {
                        "addfundsstrategy" => new AddFundsStrategy(),
                        "subtractfundsstrategy" => new SubtractFundsStrategy(),
                        "forcesubtractfundsstrategy" => new ForceSubtractFundsStrategy(),
                        _ => throw new ArgumentException("Invalid strategy")
                        };

                await _walletService.AdjustBalanceAsync(walletId, amount, currency, balanceStrategy);

                return Ok("Balance adjusted successfully");
                }
            catch (InvalidOperationException ex) when (ex.Message == "Insufficient funds")
                {
                // Return 400 if not enough funds for subtraction
                return BadRequest("Insufficient funds");
                }
            catch (Exception)
                {
                // Return 500 for any other error
                return StatusCode(500, "An error occurred while adjusting the balance");
                }
            }
        }
    }