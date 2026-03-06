using System.Text.Json.Serialization;

namespace HevySharp.Schemas;

public class HevySet
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("weight_kg")]
    public double? WeightKg { get; set; }

    [JsonPropertyName("reps")]
    public int? Reps { get; set; }
}
