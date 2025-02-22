namespace CustomersService.Application.Exceptions;

public class AuthorizationFailedException(string message) : Exception(message)
{}
