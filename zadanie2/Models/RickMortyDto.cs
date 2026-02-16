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
    public int Id { get; set; }
    public string Name { get; set; } = ""; 
    public List<string> Characters { get; set; } = new();
}

public class RmResponseInfo { public string? Next { get; set; } }

public class RmPagedResponse<T> : RmResponse<T>
{
    public RmResponseInfo Info { get; set; } = new();
}

public class CharacterShortDto 
{ 
    public string Name { get; set; } = ""; 
    public string Url { get; set; } = ""; 
}

public class TopPairDto
{
    public CharacterShortDto Character1 { get; set; } = null!;
    public CharacterShortDto Character2 { get; set; } = null!;
    public int Episodes { get; set; }
}