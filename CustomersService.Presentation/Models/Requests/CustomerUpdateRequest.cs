﻿namespace CustomersService.Presentation.Models.Requests
{
    public class CustomerUpdateRequest
    {
        public Role Role { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
    }
}