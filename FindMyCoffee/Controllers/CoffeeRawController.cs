using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

[ApiController]
[Route("api/rawcoffee")]

public class CoffeeRawController : ControllerBase
{
    private readonly IConfiguration _config;

    public CoffeeRawController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        string apiKey = _config["GoogleApiKey"];
        string lat = "32.08";
        string lng = "34.78";
        string radius = "1500";
        string type = "cafe";

        string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={lat},{lng}&radius={radius}&type={type}&key={apiKey}";

        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        string json = await response.Content.ReadAsStringAsync();

        return json;
    }
}
