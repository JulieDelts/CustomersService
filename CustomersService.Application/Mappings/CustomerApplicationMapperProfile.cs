﻿using AutoMapper;
using CustomersService.Application.Models;
using CustomersService.Persistence.Entities;
using MYPBackendMicroserviceIntegrations.Messages;

namespace CustomersService.Application.Mappings;

public class CustomerApplicationMapperProfile: Profile
{
    public CustomerApplicationMapperProfile() 
    {
        CreateMap<CustomerRegistrationModel, Customer>();
        CreateMap<CustomerUpdateModel, Customer>();
        CreateMap<Customer, CustomerInfoModel>();
        CreateMap<Customer, CustomerFullInfoModel>();
        CreateMap<Customer, CustomerMessage>();
    }
}
