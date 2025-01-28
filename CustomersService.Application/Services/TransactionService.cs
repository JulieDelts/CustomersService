
using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CustomersService.Application.Services;

public class TransactionService
{
    public static readonly HttpClient _httpClient = new HttpClient();

    public TransactionService()
    {
        _httpClient.BaseAddress = new Uri("http://194.147.90.249:9091/api/"); //http://194.147.90.249:9091/swagger/index.html
        _httpClient.Timeout = new TimeSpan(0, 0, 30);
    }

    public async Task<AccountBalanceResponse> GetBalanceByIdAsync(Guid id) 
    {
        var response = await _httpClient.GetAsync($"v1/accounts/{id}/balance");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var balanceModel = JsonSerializer.Deserialize<AccountBalanceResponse>(content);
        return balanceModel;
    }
    public async Task<TransactionResponse> GetTransactionsByAccountIdAsync(Guid accountid)
    {
        var response = await _httpClient.GetAsync($"v1/accounts/{accountid}/transactions");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var transactionsModel = JsonSerializer.Deserialize<TransactionResponse>(content);
        return transactionsModel;
    }
    public async Task<TransactionResponse> GetTransactionByTransactionIdAsync(Guid transactionId)
    {
        var response = await _httpClient.GetAsync($"v1/transactions/{transactionId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var transactionModel = JsonSerializer.Deserialize<TransactionResponse>(content);
        return transactionModel;
    }

    public async Task<Guid> CreateTransaction(CreateTransactionRequest transactionCreation, TransactionType transactionType) 
    {
        var transaction = JsonSerializer.Serialize(transactionCreation);
        var requestTransaction = new StringContent(transaction, Encoding.UTF8, "aplication/json");
        string methodLink;
        if (transactionType == TransactionType.Deposit)
            methodLink = "v1/transactions/deposit";

        else if (transactionType == TransactionType.Withdrawal)
            methodLink = "v1/transactions/withdraw";
        else return new Guid(); //to do  ввести exception 

        var response = await _httpClient.PostAsync(methodLink, requestTransaction);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return new Guid(content);
    }
    public async Task<Guid> CreateTransactionTransfer(CreateTransferTransactionRequest createtransferTransactionRequest, TransactionType transactionType)
    {
        var transaction = JsonSerializer.Serialize(createtransferTransactionRequest);
        var requestTransaction = new StringContent(transaction, Encoding.UTF8, "aplication/json");
        string methodLink = "/api/v1/transactions/transfer";
        

        var response = await _httpClient.PostAsync(methodLink, requestTransaction);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        
        return new Guid(content);
    }
} 

