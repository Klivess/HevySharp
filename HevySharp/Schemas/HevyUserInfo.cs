using System.Text.Json.Serialization;

namespace HevySharp.Schemas;

public class HevyUserInfo
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }

    [JsonPropertyName("weight_unit")]
    public string? WeightUnit { get; set; }

    [JsonPropertyName("distance_unit")]
    public string? DistanceUnit { get; set; }
}
