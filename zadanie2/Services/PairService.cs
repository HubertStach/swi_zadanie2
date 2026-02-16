using Zadanie2.Models;
using System.Text.Json;

namespace Zadanie2.Services;

public class PairsService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public PairsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TopPairDto>> GetTopPairsAsync(int? min, int? max, int limit = 20)
    {
        var allEpisodes = await GetAllEpisodesAsync();

       
        //slownik - klucz to para id obu postaci a wartosc to liczba wspolnych odcinkow
        var pairCounts = new Dictionary<(int, int), int>();

        foreach (var episode in allEpisodes)
        {
            var characterIds = episode.Characters
                .Select(url => int.Parse(url.Split('/').Last()))
                .OrderBy(id => id)
                .ToList();
            
            //generujemy wszystkie kombinacje z odcinku
            for (int i = 0; i < characterIds.Count; i++)
            {
                for (int j = i + 1; j < characterIds.Count; j++)
                {
                    var pair = (characterIds[i], characterIds[j]);
                    pairCounts[pair] = pairCounts.GetValueOrDefault(pair) + 1;
                }
            }
        }
        
        var filteredPairs = pairCounts
            .Where(p => (!min.HasValue || p.Value >= min.Value) && (!max.HasValue || p.Value <= max.Value))
            .OrderByDescending(p => p.Value)
            .Take(limit)
            .ToList();

        if (!filteredPairs.Any()) return new List<TopPairDto>();

        //pobieramy dane postaci ktore sa w topie wynikow
        var uniqueCharacterIds = filteredPairs.SelectMany(p => new[] { p.Key.Item1, p.Key.Item2 }).Distinct();
        var characterData = await GetCharactersByIdsAsync(uniqueCharacterIds);
        
        //skladamy wynik
        return filteredPairs.Select(p => new TopPairDto
        {
            Character1 = characterData[p.Key.Item1],
            Character2 = characterData[p.Key.Item2],
            Episodes = p.Value
        }).ToList();
    }

    private async Task<List<RmEpisode>> GetAllEpisodesAsync()
    {
        var results = new List<RmEpisode>();
        string? nextUrl = "episode";

        while (nextUrl != null)
        {
            var response = await _httpClient.GetFromJsonAsync<RmPagedResponse<RmEpisode>>(nextUrl, _options);
            if (response?.Results != null) results.AddRange(response.Results);
            nextUrl = response?.Info?.Next;
        }
        return results;
    }

    private async Task<Dictionary<int, CharacterShortDto>> GetCharactersByIdsAsync(IEnumerable<int> ids)
    {
        //pobieranie kilku postaci na raz
        var idsString = string.Join(",", ids);
        var response = await _httpClient.GetAsync($"character/{idsString}");
        var content = await response.Content.ReadAsStringAsync();
        
        var characters = JsonSerializer.Deserialize<List<RmCharacter>>(content, _options);

        return characters!.ToDictionary(
            c => int.Parse(c.Url.Split('/').Last()),
            c => new CharacterShortDto { Name = c.Name, Url = c.Url }
        );
    }
}