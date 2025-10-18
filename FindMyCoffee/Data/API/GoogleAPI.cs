using System.Text.Json.Serialization;

namespace FindMyCoffee.Data.API;
public class PlacesResults
{
    [JsonPropertyName("results")]
    public List<Info> Result { get; set; }
}

public class Info
{
    [JsonPropertyName("geometry")]
    public Locations Geometry { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("opening_hours")]
    public Activity OpeningHours { get; set; }

    [JsonPropertyName("photos")]
    public List<PhotoInfo>? Photos { get; set; }

    [JsonPropertyName("photo_reference")]
    public string PhotoReference { get; set; }

    [JsonPropertyName("place_id")]
    public string? PlaceId { get; set; }

    [JsonPropertyName("price_level")]
    public int PriceLevel { get; set; }

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("types")]
    public List<string> Types { get; set; }

    [JsonPropertyName("user_ratings_total")]
    public int TotalRating { get; set; }

    [JsonPropertyName("vicinity")]
    public string Vicinity { get; set; }
}

public class Locations
{
    [JsonPropertyName("location")]
    public Coordinates Location { get; set; }
}

public class Coordinates
{
    [JsonPropertyName("lat")]
    public double lat { get; set; }
    [JsonPropertyName("lng")]
    public double lng { get; set; }
}

public class Activity
{
    [JsonPropertyName("open_now")]
    public bool IsOpen { get; set; }
}

public class PhotoInfo
{
    [JsonPropertyName("html_attributions")]
    public List<string>? HtmlAttributions { get; set; }
}   