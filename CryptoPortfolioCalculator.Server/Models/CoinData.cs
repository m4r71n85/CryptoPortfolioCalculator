using Newtonsoft.Json;

public class CoinData
{
    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("price_usd")]
    public decimal PriceUsd { get; set; }
}

public class CoinDataResponse
{
    [JsonProperty("data")]
    public CoinData[] Data { get; set; }
}