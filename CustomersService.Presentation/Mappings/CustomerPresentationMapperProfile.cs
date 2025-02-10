using AutoMapper;
using CustomersService.Application.Models;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;

namespace CustomersService.Presentation.Mappings
{
    public class CustomerPresentationMapperProfile: Profile
    {
        public CustomerPresentationMapperProfile() 
        {
            CreateMap<CustomerInfoModel, CustomerResponse>();
            CreateMap<CustomerFullInfoModel, CustomerFullResponse>();
            CreateMap<RegisterCustomerRequest, CustomerRegistrationModel>();
            CreateMap<CustomerUpdateRequest, CustomerUpdateModel>();
        }
    }
}
