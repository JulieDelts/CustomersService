﻿using AutoMapper;
using CustomersService.Application.Models;
using CustomersService.Persistence.Entities;
using MYPBackendMicroserviceIntegrations.Messages;

namespace CustomersService.Application.Mappings;

public class AccountApplicationMapperProfile: Profile
{
    public AccountApplicationMapperProfile() 
    {
        CreateMap<AccountCreationModel, Account>();
        CreateMap<Account, AccountInfoModel>();
        CreateMap<Account, AccountFullInfoModel>();
        CreateMap<Account, AccountMessage>();
    }
}
