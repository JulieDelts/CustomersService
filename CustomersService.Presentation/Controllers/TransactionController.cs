using AutoMapper;
using CustomersService.Application.Interfaces;
using CustomersService.Core.IntegrationModels.Requests;
using CustomersService.Core.IntegrationModels.Responses;
using CustomersService.Core.Enum;
using CustomersService.Presentation.Configuration;
using CustomersService.Presentation.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionController(
        ITransactionService transactionService,
        IMapper mapper) : ControllerBase
    {
        [HttpPost("deposit")]
        public async Task<ActionResult<Guid>> CreateDepositTransactionAsync([FromBody] TransactionCreateRequest request)
        {
            var customerId = this.GetCustomerIdFromClaims();
            var transactionModel = mapper.Map<CreateTransactionRequest>(request);
            var transactionId = await transactionService.CreateSimpleTransactionAsync(transactionModel, customerId, TransactionType.Deposit);
            return Ok(transactionId);
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<Guid>> CreateWithdrawTransactionAsync([FromBody] TransactionCreateRequest request)
        {
            var customerId = this.GetCustomerIdFromClaims();
            var transactionModel = mapper.Map<CreateTransactionRequest>(request);
            var transactionId = await transactionService.CreateSimpleTransactionAsync(transactionModel, customerId, TransactionType.Withdrawal);
            return Ok(transactionId);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<List<Guid>>> CreateTransferTransactionAsync([FromBody] TransferTransactionCreateRequest request)
        {
            var customerId = this.GetCustomerIdFromClaims();
            var transactionModel = mapper.Map<CreateTransferTransactionRequest>(request);
            var transactionIds = await transactionService.CreateTransferTransactionAsync(transactionModel, customerId);
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
