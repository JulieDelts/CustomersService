
using Moq;
using Moq.Protected;
using System.Net;

namespace CustomersService.Application.Tests
{
    public static class HttpMessageHandlerMockSetup
    {
        public static void SetupProtectedHttpMessageHandlerMock(Mock<HttpMessageHandler> messageHandlerMock, HttpStatusCode httpStatusCode, string? response = null)
        {
            var responseMessage = new HttpResponseMessage { StatusCode = httpStatusCode };

            if (httpStatusCode == HttpStatusCode.OK)
                responseMessage.Content = new StringContent(response);

            var mockProtected = messageHandlerMock.Protected();
            var setupApiRequest = mockProtected
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);
        }
    }
}
