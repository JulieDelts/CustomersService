using AutoMapper;
using CustomersService.Application.Models;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;

namespace CustomersService.Presentation.Mappings;
public class AccountPresentationMapperProfile : Profile
{
    public AccountPresentationMapperProfile()
    {
        CreateMap<AccountInfoModel, AccountResponse>();
        CreateMap<AccountFullInfoModel,AccountFullInfoResponse>();
        CreateMap<AccountCreationModel, AccountInfoModel>();
        CreateMap<AccountAddRequest, AccountCreationModel>();
    }
}
