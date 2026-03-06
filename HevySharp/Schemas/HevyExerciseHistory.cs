using System.Text.Json.Serialization;

namespace HevySharp.Schemas;

public class HevyExerciseHistory
{
    [JsonPropertyName("exercise_template_id")]
    public string? ExerciseTemplateId { get; set; }

    [JsonPropertyName("history")]
    public List<HevyExerciseHistoryEntry>? History { get; set; }
}

public class HevyExerciseHistoryEntry
{
    [JsonPropertyName("workout_id")]
    public string? WorkoutId { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("sets")]
    public List<HevySet>? Sets { get; set; }
}
