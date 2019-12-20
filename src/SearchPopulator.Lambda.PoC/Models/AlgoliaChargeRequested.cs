using System;
using Finance.Impact.Events.Gateway;
using Newtonsoft.Json;

namespace SearchPopulator.Lambda.PoC.Models
{
    public class AlgoliaChargeRequested : ChargeRequested
    {
        [JsonProperty("objectID")] public string ObjectId { get; set; } = Guid.NewGuid().ToString();
    }
}