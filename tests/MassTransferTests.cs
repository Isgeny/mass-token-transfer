using MassTokenTransfer.Tests.Fixture;

namespace MassTokenTransfer.Tests;

public class MassTransferTests
{
    private readonly MassTokenTransferAccount _massTokenTransferAccount;

    public MassTransferTests()
    {
        _massTokenTransferAccount = new MassTokenTransferAccount();
    }

    [Fact]
    public void MassTransfer_OneRecipient_Success()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
        var account1 = PrivateNode.GenerateAccount(0);

        var transactionId = _massTokenTransferAccount.InvokeMassTransfer(sender, 10_00000000, new List<MassTransferItem>
        {
            new() { Recipient = account1.GetAddress().Encoded, Amount = 10_00000000, Asset = "WAVES" },
        });

        using (new AssertionScope())
        {
            transactionId.Should().NotBeEmpty();

            PrivateNode.Instance.GetBalance(sender.GetAddress()).Should().Be(89_99500000);
            PrivateNode.Instance.GetBalance(account1.GetAddress()).Should().Be(10_00000000);
        }
    }

    [Fact]
    public void MassTransfer_TwoRecipients_Success()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
        var account1 = PrivateNode.GenerateAccount(0);
        var account2 = PrivateNode.GenerateAccount(0);

        var transactionId = _massTokenTransferAccount.InvokeMassTransfer(sender, 50_00000000, new List<MassTransferItem>
        {
            new() { Recipient = account1.GetAddress().Encoded, Amount = 10_00000000, Asset = "WAVES" },
            new() { Recipient = account2.GetAddress().Encoded, Amount = 40_00000000, Asset = "WAVES" },
        });

        using (new AssertionScope())
        {
            transactionId.Should().NotBeEmpty();

            PrivateNode.Instance.GetBalance(sender.GetAddress()).Should().Be(49_99500000);
            PrivateNode.Instance.GetBalance(account1.GetAddress()).Should().Be(10_00000000);
            PrivateNode.Instance.GetBalance(account2.GetAddress()).Should().Be(40_00000000);
        }
    }

    [Fact]
    public void MassTransfer_HundredRecipients_Success()
    {
        var sender = PrivateNode.GenerateAccount(101_00000000);

        var accounts = Enumerable.Range(0, 100).Select(_ => PrivateNode.GenerateAccount(0)).ToList();
        var massTransferItems = accounts.Select(x => new MassTransferItem { Recipient = x.GetAddress().Encoded, Amount = 1_00000000, Asset = "WAVES" }).ToList();

        var transactionId = _massTokenTransferAccount.InvokeMassTransfer(sender, 100_00000000, massTransferItems);

        using (new AssertionScope())
        {
            transactionId.Should().NotBeEmpty();

            PrivateNode.Instance.GetBalance(sender.GetAddress()).Should().Be(99500000);

            foreach (var account in accounts)
            {
                PrivateNode.Instance.GetBalance(account.GetAddress()).Should().Be(1_00000000);
            }
        }
    }
}