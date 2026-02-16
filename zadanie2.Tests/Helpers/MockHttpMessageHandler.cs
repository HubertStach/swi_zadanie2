using System.Net;
using Moq;
using Moq.Protected;

namespace zadanie2.Tests.Helpers;

public static class MockHttpMessageHandler
{
    public static Mock<HttpMessageHandler> SetupMockRequest(string urlPart, string jsonResponse)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains(urlPart)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });
        return handlerMock;
    }
}