namespace SmartHome.PrivateOffice.Pes.Models;

internal record PesAccount
{
    public int AccountId { get; init; }
    public int ProviderId { get; init; }
    public string ProviderName { get; init; } = "";
    public int ServiceId { get; init; }
    public string ServiceName { get; init; } = "";
    public int ObjectId { get; init; }
    public string ObjectName { get; init; } = "";
    public List<PesDisplayKey> AccountDisplayKey { get; init; } = new();
    public string AccountAlias { get; init; } = "";
    public bool AllowToCreateAutoPayment { get; init; }
    public bool AllowToSelectInvoiceMethod { get; init; }
    public bool AutoPaymentEnabled { get; init; }
    public string DeliveryType { get; init; } = "";
    public bool HasHomeKit { get; init; }
}