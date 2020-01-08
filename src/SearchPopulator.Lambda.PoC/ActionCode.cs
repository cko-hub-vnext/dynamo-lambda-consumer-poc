namespace SearchPopulator.Lambda.PoC
{
    public enum ActionCode
    {
        Refund = 2,
        Authorize = 4,
        Capture = 5,
        VoidAuthorize = 9,
        Expiry = 17,
        CardVerification = 30,
        Chargeback = 50,
        FailedAuthorize = 90,
        FailedAuthentication = 91,
        PayoutAuthorized = 100,
        PayoutDeclined = 101
    }
}