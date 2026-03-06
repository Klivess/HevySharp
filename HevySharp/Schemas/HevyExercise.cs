using System.Text.Json.Serialization;

namespace HevySharp.Schemas;

public class HevyExercise
{
    [JsonPropertyName("exercise_template_id")]
    public string? ExerciseTemplateId { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("superset_id")]
    public string? SupersetId { get; set; }

    [JsonPropertyName("rest_seconds")]
    public int? RestSeconds { get; set; }

    [JsonPropertyName("sets")]
    public List<HevySet>? Sets { get; set; }
}
