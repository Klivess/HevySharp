using System.Text.Json.Serialization;

namespace HevySharp.Schemas;

public class HevyExerciseTemplate
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("muscle_group")]
    public string? MuscleGroup { get; set; }

    [JsonPropertyName("equipment_category")]
    public string? EquipmentCategory { get; set; }

    [JsonPropertyName("is_custom")]
    public bool? IsCustom { get; set; }
}
