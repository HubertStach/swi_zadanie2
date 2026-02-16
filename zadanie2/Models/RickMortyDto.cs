namespace Zadanie2.Models;

public class RmResponse<T>
{
    public List<T> Results { get; set; } = new();
}

public class RmCharacter
{
    public string Name { get; set; } = ""; 
    public string Url { get; set; } = "";
}

public class RmLocation
{
    public string Name { get; set; } = ""; 
    public string Url { get; set; } = "";
}

public class RmEpisode
{
    public string Name { get; set; } = ""; 
    public string Url { get; set; } = "";
}