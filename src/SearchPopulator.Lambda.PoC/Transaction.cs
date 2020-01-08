using System;
using Newtonsoft.Json;

namespace SearchPopulator.Lambda.PoC
{
    public class Transaction
    {
        [JsonProperty("pk")]
        public string PartitionKey => PaymentId.ToString();

        [JsonProperty("sk")]
        public DateTime SortKey => Timestamp;

        public DateTime Timestamp { get; set; }
        public Guid? ActionId { get; set; }
        public Guid? CustomerToken { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? SourceId { get; set; }
        public bool Deferred { get; set; }
        public bool IsCascaded { get; set; }
        public bool? IsLive { get; set; }
        public decimal RevenueAdjustment { get; set; }
        public decimal Value { get; set; }
        public int AppliedTo { get; set; }
        public int? ActionCodeId { get; set; }
        public int? ChargeModeCode { get; set; }
        public int? CurrencyId { get; set; }
        public int? LastSuccessfulActionCodeId { get; set; }
        public long BusinessId { get; set; }
        public long ChannelId { get; set; }
        public long MerchantId { get; set; }
        public string AcquirerAuthCode { get; set; }
        public string AcquirerReferenceNumber { get; set; }
        public string ActionDisplayId { get; set; }
        public string ApiVersion { get; set; }
        public string BCTransactionId { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingCountry { get; set; }
        public string BillingPhone { get; set; }
        public string BillingPhoneCountryCode { get; set; }
        public string BillingPostcode { get; set; }
        public string BillingState { get; set; }
        public string BillingDescriptor { get; set; }
        public string CardAvsCheck { get; set; }
        public string CardCvvCheck { get; set; }
        public string CardExpMonth { get; set; }
        public string CardExpYear { get; set; }
        public string CardHolderName { get; set; }
        public string CardIssuingBank { get; set; }
        public string CardIssuingCountry { get; set; }
        public string CardNumber { get; set; }
        public string CardType { get; set; }
        public string CardWalletType { get; set; }
        public string ChargeId { get; set; }
        public string ChargebackIndicator { get; set; }
        public string CurrencyCode { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerDisplayId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerIp { get; set; }
        public string Description { get; set; }
        public string MandateReference { get; set; }
        public string PaymentDisplayId { get; set; }
        public string PaymentMethod { get; set; }
        public string Product { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseSummary { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string ShippingAddressLine2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingPhoneCountryCode { get; set; }
        public string ShippingPostcode { get; set; }
        public string ShippingState { get; set; }
        public string Status { get; set; }
        public string TrackId { get; set; }
        public string TransactionType { get; set; }
        public string UDF1 { get; set; }

        // new account structure properties
        public string ClientId { get; set; }
        public string EntityId { get; set; }
        public string ProcessingChannelId { get; set; }
    }
}