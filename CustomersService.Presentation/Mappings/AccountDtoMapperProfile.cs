using AutoMapper;
using CustomersService.Application.Models;
using CustomersService.Presentation.Models.Responses;

namespace CustomersService.Presentation.Mappings;
public class AccountDtoMapperProfile : Profile
{
    public AccountDtoMapperProfile()
    {
        CreateMap<AccountInfoModel, AccountResponse>();
        CreateMap<AccountCreationModel, AccountInfoModel>();
    }
}
