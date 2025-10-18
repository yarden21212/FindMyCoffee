using System.Text.Json.Serialization;

namespace FindMyCoffee.Data.HttpResponses
{
    public class DistanceMatrixResponse
    {
        [JsonPropertyName("rows")]
        public List<ElementsData> Rows {set; get;} 
    }
    public class ElementsData
    {
        [JsonPropertyName("elements")]
        public List<ArrivalData> Elements { set; get; }
    }
    public class ArrivalData
    {
        [JsonPropertyName("distance")]
        public DistanceData DistanceData { set; get; }

        [JsonPropertyName("duration")]
        public DurationData DurationData { set; get; }
    }

    public class DistanceData
    {
        [JsonPropertyName("value")]
        public int Distance { get; set; }
    }
    public class DurationData
    {
        [JsonPropertyName("value")]
        public int Duration { get; set; }
    }


}
