using System.Text.Json.Serialization;

namespace HevySharp.Schemas;

public class HevyRoutine
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("folder_id")]
    public string? FolderId { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("updated_at")]
    public string? UpdatedAt { get; set; }

    [JsonPropertyName("exercises")]
    public List<HevyExercise>? Exercises { get; set; }
}
