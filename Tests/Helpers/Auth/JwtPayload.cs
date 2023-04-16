using Newtonsoft.Json;

namespace Tests;

internal class JwtPayload
{
    [JsonProperty("sub")]
    public string Sub { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("u_typ")]
    public string UTyp { get; set; }
}