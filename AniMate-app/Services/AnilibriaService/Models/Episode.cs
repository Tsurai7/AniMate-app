using Newtonsoft.Json;

namespace AniMate_app.Services.AnilibriaService.Models
{
    public record Episode
    {
        private const string _url = "https://www.anilibria.tv";

        [JsonConstructor]
        public Episode(string preview)
        {
            Preview = $"{_url}{preview}";
        }

        [JsonProperty("episode")]
        public string Ordinal { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("hls")]
        public Hls HlsUrls { get; set; }

        [JsonProperty("created_timestamp")]
        public int CreatedTimestamp { get; set; }

        [JsonProperty("preview")]
        public string Preview { get; set; }
    }

    public record Hls
    {
        private const string BaseAdress = "https://cache.libria.fun";

        [JsonConstructor]
        public Hls(string fhd, string hd, string sd)
        {
            Fhd = $"{BaseAdress}{fhd}";
            Hd = $"{BaseAdress}{hd}";
            Sd = $"{BaseAdress}{sd}";
        }

        [JsonProperty("fhd")]
        public string Fhd { get; set; }

        [JsonProperty("hd")]
        public string Hd { get; set; }

        [JsonProperty("sd")]
        public string Sd { get; set; }
    }
}
