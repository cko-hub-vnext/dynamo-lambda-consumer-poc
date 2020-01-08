using System;
using Newtonsoft.Json;

namespace SearchPopulator.Lambda.PoC
{
    public class HubRecord
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        public string PaymentId { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyIso { get; set; }

        public string CardType { get; set; }

        public string Action { get; set; }

        public int Bin { get; set; }

        public short LastFourCardDigits { get; set; }

        public string Reference { get; set; }

        public string CardholderName { get; set; }

        public string Email { get; set; }

        public string StatusCode { get; set; }

        public DateTime TimeStamp { get; set; }

        public string ActionId { get; set; }

        public string Type { get; set; }

        public HubRecord Clone()
        {
            return (HubRecord)MemberwiseClone();
        }
    }
}