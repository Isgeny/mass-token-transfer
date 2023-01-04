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
    public void OneRecipient_OneAsset_Success()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
        var recipient = PrivateNode.GenerateAccount(0);

        var payments = new List<Amount>
        {
            new() { Value = 10_00000000, },
        };
        var massTransferItems = new List<MassTransferItem>
        {
            new() { Recipient = recipient.GetAddress().Encoded, Amount = 10_00000000, AssetIdx = 0, },
        };

        var transactionId = _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        using (new AssertionScope())
        {
            transactionId.Should().NotBeEmpty();

            PrivateNode.Instance.GetBalance(sender.GetAddress()).Should().Be(89_99500000);
            PrivateNode.Instance.GetBalance(recipient.GetAddress()).Should().Be(10_00000000);
        }
    }

    [Fact]
    public void OneRecipient_TwoAssets_Success()
    {
        var sender = PrivateNode.GenerateAccount(101_00000000);
        var assetId = PrivateNode.IssueAsset(sender, "TOKEN", 10_000000, 6);
        var recipient = PrivateNode.GenerateAccount(0);

        var payments = new List<Amount>
        {
            new() { Value = 10_00000000, },
            new() { Value = 8_000000, AssetId = assetId, },
        };
        var massTransferItems = new List<MassTransferItem>
        {
            new() { Recipient = recipient.GetAddress().Encoded, Amount = 10_00000000, AssetIdx = 0, },
            new() { Recipient = recipient.GetAddress().Encoded, Amount = 8_000000, AssetIdx = 1, },
        };

        var transactionId = _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        using (new AssertionScope())
        {
            transactionId.Should().NotBeEmpty();

            PrivateNode.Instance.GetBalance(sender.GetAddress()).Should().Be(89_99500000);
            PrivateNode.Instance.GetAssetBalance(sender.GetAddress(), assetId).Should().Be(2_000000);
            PrivateNode.Instance.GetBalance(recipient.GetAddress()).Should().Be(10_00000000);
            PrivateNode.Instance.GetAssetBalance(recipient.GetAddress(), assetId).Should().Be(8_000000);
        }
    }

    [Fact]
    public void TwoRecipients_OneAsset_Success()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
        var recipient1 = PrivateNode.GenerateAccount(0);
        var recipient2 = PrivateNode.GenerateAccount(0);

        var payments = new List<Amount>
        {
            new() { Value = 50_00000000, },
        };
        var massTransferItems = new List<MassTransferItem>
        {
            new() { Recipient = recipient1.GetAddress().Encoded, Amount = 10_00000000, AssetIdx = 0, },
            new() { Recipient = recipient2.GetAddress().Encoded, Amount = 40_00000000, AssetIdx = 0, },
        };

        var transactionId = _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        using (new AssertionScope())
        {
            transactionId.Should().NotBeEmpty();

            PrivateNode.Instance.GetBalance(sender.GetAddress()).Should().Be(49_99500000);
            PrivateNode.Instance.GetBalance(recipient1.GetAddress()).Should().Be(10_00000000);
            PrivateNode.Instance.GetBalance(recipient2.GetAddress()).Should().Be(40_00000000);
        }
    }

    [Fact]
    public void TwoRecipients_TwoAssets_Success()
    {
        var sender = PrivateNode.GenerateAccount(101_00000000);
        var assetId = PrivateNode.IssueAsset(sender, "TOKEN", 10_000000, 6);
        var recipient1 = PrivateNode.GenerateAccount(0);
        var recipient2 = PrivateNode.GenerateAccount(0);

        var payments = new List<Amount>
        {
            new() { Value = 50_00000000, },
            new() { Value = 8_000000, AssetId = assetId, },
        };
        var massTransferItems = new List<MassTransferItem>
        {
            new() { Recipient = recipient1.GetAddress().Encoded, Amount = 10_00000000, AssetIdx = 0, },
            new() { Recipient = recipient1.GetAddress().Encoded, Amount = 8_000000, AssetIdx = 1, },
            new() { Recipient = recipient2.GetAddress().Encoded, Amount = 40_00000000, AssetIdx = 0, },
        };

        var transactionId = _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        using (new AssertionScope())
        {
            transactionId.Should().NotBeEmpty();

            PrivateNode.Instance.GetBalance(sender.GetAddress()).Should().Be(49_99500000);
            PrivateNode.Instance.GetAssetBalance(sender.GetAddress(), assetId).Should().Be(2_000000);
            PrivateNode.Instance.GetBalance(recipient1.GetAddress()).Should().Be(10_00000000);
            PrivateNode.Instance.GetAssetBalance(recipient1.GetAddress(), assetId).Should().Be(8_000000);
            PrivateNode.Instance.GetBalance(recipient2.GetAddress()).Should().Be(40_00000000);
            PrivateNode.Instance.GetAssetBalance(recipient2.GetAddress(), assetId).Should().Be(0);
        }
    }

    [Fact]
    public void HundredRecipients_OneAsset_Success()
    {
        var sender = PrivateNode.GenerateAccount(101_00000000);
        var recipients = Enumerable.Range(0, 100).Select(_ => PrivateNode.GenerateAccount(0)).ToList();

        var payments = new List<Amount>
        {
            new() { Value = 100_00000000, },
        };
        var massTransferItems = recipients.Select(x => new MassTransferItem
        {
            Recipient = x.GetAddress().Encoded, Amount = 1_00000000, AssetIdx = 0,
        }).ToList();

        var transactionId = _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        using (new AssertionScope())
        {
            transactionId.Should().NotBeEmpty();

            PrivateNode.Instance.GetBalance(sender.GetAddress()).Should().Be(0_99500000);

            recipients.Should().AllSatisfy(x => PrivateNode.Instance.GetBalance(x.GetAddress()).Should().Be(1_00000000));
        }
    }

    [Fact]
    public void HundredRecipients_TwoAssets_Success()
    {
        var sender = PrivateNode.GenerateAccount(102_00000000);
        var assetId = PrivateNode.IssueAsset(sender, "TOKEN", 10_000000, 6);
        var recipients = Enumerable.Range(0, 50).Select(_ => PrivateNode.GenerateAccount(0)).ToList();

        var payments = new List<Amount>
        {
            new() { Value = 100_00000000, },
            new() { Value = 10_000000, AssetId = assetId, },
        };
        var massTransferItems = recipients.Select(x => new MassTransferItem
        {
            Recipient = x.GetAddress().Encoded, Amount = 2_00000000, AssetIdx = 0,
        }).Concat(recipients.Select(x => new MassTransferItem
        {
            Recipient = x.GetAddress().Encoded, Amount = 0_200000, AssetIdx = 1,
        })).ToList();

        var transactionId = _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        using (new AssertionScope())
        {
            transactionId.Should().NotBeEmpty();

            PrivateNode.Instance.GetBalance(sender.GetAddress()).Should().Be(0_99500000);
            PrivateNode.Instance.GetAssetBalance(sender.GetAddress(), assetId).Should().Be(0);
            recipients.Should().AllSatisfy(x =>
            {
                PrivateNode.Instance.GetBalance(x.GetAddress()).Should().Be(2_00000000);
                PrivateNode.Instance.GetAssetBalance(x.GetAddress(), assetId).Should().Be(0_200000);
            });
        }
    }
}