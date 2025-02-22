using AutoMapper;
using CustomersService.Core.IntegrationModels.Requests;
using CustomersService.Presentation.Models.Requests;

namespace CustomersService.Presentation.Mappings;

public class TransactionPresentationMapperProfile: Profile
{
    public TransactionPresentationMapperProfile() 
    {
        CreateMap<TransferTransactionCreateRequest, CreateTransferTransactionRequest>();
        CreateMap<TransactionCreateRequest, CreateTransactionRequest>();
    }
}
