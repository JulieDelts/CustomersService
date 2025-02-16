using CustomersService.Application.Interfaces;
using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using CustomersService.Presentation.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionController(
        ITransactionService transactionService,
        ILogger<TransactionController> logger) : ControllerBase
    {
        [HttpPost("deposit")]
        public async Task<ActionResult<Guid>> CreateDepositTransactionAsync([FromBody] CreateTransactionRequest request)
        {
            var customerId = this.GetCustomerIdFromClaims();
            var transactionId = await transactionService.CreateSimpleTransactionAsync(request, customerId, TransactionType.Deposit);
            return Ok(transactionId);
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<Guid>> CreateWithdrawTransactionAsync([FromBody] CreateTransactionRequest request)
        {
            var customerId = this.GetCustomerIdFromClaims();
            var transactionId = await transactionService.CreateSimpleTransactionAsync(request, customerId, TransactionType.Withdrawal);
            return Ok(transactionId);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<List<Guid>>> CreateTransferTransactionAsync([FromBody] CreateTransferTransactionRequest request)
        {
            var customerId = this.GetCustomerIdFromClaims();
            var transactionIds = await transactionService.CreateTransferTransactionAsync(request, customerId);
            return Ok(transactionIds);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponse>> GetByIdAsync([FromRoute] Guid id)
        {
            var transaction = await transactionService.GetByIdAsync(id);
            return Ok(transaction);
        }
    }
}
