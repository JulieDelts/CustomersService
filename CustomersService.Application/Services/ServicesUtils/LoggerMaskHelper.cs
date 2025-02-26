namespace CustomersService.Application.Services.ServicesUtils;

public static class LoggerMaskHelper
{
    public static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 1)
        {
            return email;
        }

        var maskedEmail = email.Substring(0, 1) + new string('*', atIndex - 1) + email.Substring(atIndex);
        return maskedEmail;
    }
}
