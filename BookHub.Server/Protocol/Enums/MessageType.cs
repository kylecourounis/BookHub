namespace BookHub.Server.Protocol.Enums
{
    internal enum MessageType
    {
        // -- Client --
        Login                     = 1001,
        AddListing                = 1002,
        Search                    = 1003,
        BuyListing                = 1004,
        UpdateAccountInfo         = 1005,
        GetColleges               = 1006,

        // --- Server ---
        LoginResponse             = 2001,
        AddListingResponse        = 2002,
        SearchResponse            = 2003,
        BuyListingResponse        = 2004,
        UpdateAccountInfoResponse = 2005,
        GetCollegesResponse       = 2006,

        // --------------
        None = -1
    }
}
