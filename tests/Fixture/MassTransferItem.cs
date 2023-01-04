namespace MassTokenTransfer.Tests.Fixture;

public record MassTransferItem
{
    public string Recipient { get; set; } = string.Empty;

    public long Amount { get; set; }
}