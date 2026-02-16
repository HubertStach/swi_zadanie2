using FluentAssertions;
using Zadanie2.Services;
using System.Net;
using Moq;
using Moq.Protected;

namespace zadanie2.Tests;

public class SearchServiceTests
{
    [Fact]
    public async Task SearchAllAsync_ShouldCombineResults()
    {
        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>();
        var emptyResponse = "{ \"results\": [] }";
        var characterResponse = "{ \"results\": [{ \"name\": \"Rick Sanchez\", \"url\": \"url1\" }] }";

        handlerMock.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(characterResponse) })
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(emptyResponse) })
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(emptyResponse) });

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://test.com/api/") };
        var service = new SearchService(httpClient);

        // ACT
        var result = await service.SearchAllAsync("Rick", limit: 10);

        // ASSERT
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Rick Sanchez");
        result[0].Type.Should().Be("character");
    }
}