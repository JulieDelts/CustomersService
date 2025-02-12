using CustomersService.Application.Interfaces;
using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController(
        ITransactionService transactionService,
        ILogger<TransactionController> logger) : ControllerBase
    {
        [HttpPost("deposit")]
        public async Task<ActionResult<Guid>> CreateDepositTransactionAsync([FromBody] CreateTransactionRequest request)
        {
            logger.LogInformation("Received request to create deposit transaction for account {AccountId}", request.AccountId);
            logger.LogTrace("Request data: {@Request}", request);

            var transactionId = await transactionService.CreateDepositTransactionAsync(request);
            logger.LogInformation("Transaction created successfully with Id {TransactionId}", transactionId);

            return Ok(transactionId);
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<Guid>> CreateWithdrawTransactionAsync([FromBody] CreateTransactionRequest request)
        {
            logger.LogInformation("Received request to create deposit transaction for account {AccountId}", request.AccountId);
            logger.LogTrace("Request data: {@Request}", request);

            var transactionId = await transactionService.CreateWithdrawTransactionAsync(request);
            logger.LogInformation("Transaction created successfully with Id {TransactionId}", transactionId);

            return Ok(transactionId);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<List<Guid>>> CreateTransferTransactionAsync([FromBody] CreateTransferTransactionRequest request)
        {
            logger.LogInformation("Received request to create transfer transaction from account {FromAccountId} to account {ToAccountId}", request.FromAccountId, request.ToAccountId);
            logger.LogTrace("Request data: {@Request}", request);

            var transactionIds = await transactionService.CreateTransferTransactionAsync(request);
            logger.LogInformation("Transaction created successfully with Ids {TransactionIds}", transactionIds);

            return Ok(transactionIds);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponse>> GetByIdAsync([FromRoute] Guid id)
        {
            logger.LogInformation("Received request to get transaction details for transaction {TransactionId}", id);

            var transaction = await transactionService.GetByIdAsync(id);
            logger.LogTrace("Retrieved transaction: {@Transaction}", transaction);

            logger.LogInformation("Successfully retrieved transaction details for transaction {TransactionId}", id);
            return Ok(transaction);
        }
    }
}
