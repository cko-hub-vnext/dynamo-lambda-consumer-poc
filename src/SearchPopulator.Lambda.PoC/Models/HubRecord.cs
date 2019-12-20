using System;
using Algolia.Search.Utils;
using Newtonsoft.Json;

namespace SearchPopulator.Lambda.PoC.Models
{
    public class HubRecord
    {
        [JsonProperty("objectID")] public string ObjectId { get; set; } = Guid.NewGuid().ToString();

        public decimal Amount { get; set; }

        public string CurrencyIso { get; set; }

        public string CardType { get; set; }

        public string Action { get; set; }

        public string Status { get; set; }

        public string CardholderName { get; set; }

        public string Email { get; set; }

        public DateTime TimeStamp { get; set; }

        public long UnixTimestamp => TimeStamp.ToUnixTimeSeconds();

        public string CardNumber { get; set; }
    }
}