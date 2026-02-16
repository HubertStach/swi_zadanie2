using Zadanie2.Models;
using System.Text.Json;

namespace Zadanie2.Services;

public class SearchService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://rickandmortyapi.com/api/");
    }

    public async Task<List<SearchResultDto>> SearchAllAsync(string term, int? limit)
    {
        var characterTask = FetchResults<RmCharacter>("character", term, "character");
        var locationTask = FetchResults<RmLocation>("location", term, "location");
        var episodeTask = FetchResults<RmEpisode>("episode", term, "episode");

        await Task.WhenAll(characterTask, locationTask, episodeTask);

        //dodajemy wyniki i roibmy jedną listę
        var allResults = characterTask.Result
            .Concat(locationTask.Result)
            .Concat(episodeTask.Result)
            .ToList();
        
        return limit.HasValue ? allResults.Take(limit.Value).ToList() : allResults;
    }

    private async Task<List<SearchResultDto>> FetchResults<T>(string endpoint, string term, string typeLabel)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{endpoint}/?name={Uri.EscapeDataString(term)}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<SearchResultDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<RmResponse<dynamic>>(content, _jsonOptions);
            
            var results = new List<SearchResultDto>();
            if (data?.Results != null)
            {
                foreach (var item in data.Results)
                {
                    var element = JsonSerializer.Deserialize<T>(item.ToString(), _jsonOptions);
                    if (element == null) continue;

                    results.Add(new SearchResultDto
                    {
                        Name = (string)((dynamic)element).Name,
                        Url = (string)((dynamic)element).Url,
                        Type = typeLabel
                    });
                }
            }
            return results;
        }
        catch
        {
            return new List<SearchResultDto>();
        }
    }
}