using System.Text.Json.Serialization;

namespace HevySharp.Schemas;

public class HevyRoutineFolder
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}
