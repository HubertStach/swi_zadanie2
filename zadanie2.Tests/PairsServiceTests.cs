using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Zadanie2.Models;
using Zadanie2.Services;
using zadanie2.Tests.Helpers;

namespace zadanie2.Tests;

public class PairsServiceTests
{
    [Fact]
    public async Task GetTopPairsAsync_ShouldCorrectlyCalculatePairs()
    {
        // ARRANGE
        //symulacja odcinka z postaciami id 1 i 2
        var episodesJson = JsonSerializer.Serialize(new RmPagedResponse<RmEpisode>
        {
            Results = new List<RmEpisode> {
                new() { Id = 1, Name = "Pilot", Characters = new List<string> {
                    "https://rickandmortyapi.com/api/character/1",
                    "https://rickandmortyapi.com/api/character/2"
                }}
            },
            Info = new RmResponseInfo { Next = null }
        });

        //symulujemy dane postaci
        var charactersJson = JsonSerializer.Serialize(new List<RmCharacter> {
            new() { Name = "Rick", Url = ".../1" },
            new() { Name = "Morty", Url = ".../2" }
        });

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(episodesJson) })
            .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(charactersJson) });

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://test.com/api/") };
        var service = new PairsService(httpClient);

        // ACT
        var result = await service.GetTopPairsAsync(min: 1, max: 10, limit: 5);

        // ASSERT
        result.Should().NotBeEmpty();
        result[0].Episodes.Should().Be(1);
        result[0].Character1.Name.Should().Be("Rick");
        result[0].Character2.Name.Should().Be("Morty");
    }
}