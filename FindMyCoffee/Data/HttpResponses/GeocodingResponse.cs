using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

public sealed class GeocodingResponse
{
    [JsonPropertyName("results")]
    public List<Results>? Results { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

public class Results()
{
    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; set; }
}

public class Geometry
{
    [JsonPropertyName("location")]
    public Location Location { get; set; }
}

public class Location
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }
    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}
