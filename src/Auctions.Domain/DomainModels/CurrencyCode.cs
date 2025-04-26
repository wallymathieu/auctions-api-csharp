namespace Wallymathieu.Auctions.DomainModels;

/// <summary>
/// Currency code as defined by <a href="https://en.wikipedia.org/wiki/ISO_4217">ISO 4217</a>. Note that we here only have a subset of the currencies.
/// </summary>
[Serializable]
public enum CurrencyCode
{
    None,
    /// <summary>
    /// This is not a real currency code, but is used to indicate that the auction is in a virtual currency.
    /// </summary>
    VAC = 1001,
    /// <summary>
    /// Swedish Krona
    /// </summary>
    SEK = 752,
    /// <summary>
    /// Danish Krone
    /// </summary>
    DKK = 208
}