using MassTokenTransfer.Tests.Fixture;

namespace MassTokenTransfer.Tests;

public class MassTransferValidationsTests
{
    private readonly MassTokenTransferAccount _massTokenTransferAccount = new();

    [Fact]
    public void RecipientsSizeMismatch_ThrowException()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
        var recipient = PrivateNode.GenerateAccount(0);

        var payments = new List<Amount>
        {
            new() { Value = 10_00000000, },
        };

        var callArgs = new List<CallArg>
        {
            new()
            {
                Type = CallArgType.List, Value = new List<CallArg>
                {
                    new() { Type = CallArgType.ByteArray, Value = new Base64s(Base58s.Decode(recipient.GetAddress().Encoded)) },
                    new() { Type = CallArgType.ByteArray, Value = new Base64s(Base58s.Decode(recipient.GetAddress().Encoded)) },
                },
            },
            new()
            {
                Type = CallArgType.List, Value = new List<CallArg>
                {
                    new() { Type = CallArgType.Integer, Value = 10_00000000L },
                },
            },
            new()
            {
                Type = CallArgType.List, Value = new List<CallArg>
                {
                    new() { Type = CallArgType.Integer, Value = 0L },
                },
            },
            new() { Type = CallArgType.String, Value = "Test attachment" },
        };

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, callArgs);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }

    [Fact]
    public void AmountsSizeMismatch_ThrowException()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
        var recipient = PrivateNode.GenerateAccount(0);

        var payments = new List<Amount>
        {
            new() { Value = 10_00000000, },
        };

        var callArgs = new List<CallArg>
        {
            new()
            {
                Type = CallArgType.List, Value = new List<CallArg>
                {
                    new() { Type = CallArgType.ByteArray, Value = new Base64s(Base58s.Decode(recipient.GetAddress().Encoded)) },
                },
            },
            new()
            {
                Type = CallArgType.List, Value = new List<CallArg>
                {
                    new() { Type = CallArgType.Integer, Value = 5_00000000L },
                    new() { Type = CallArgType.Integer, Value = 5_00000000L },
                },
            },
            new()
            {
                Type = CallArgType.List, Value = new List<CallArg>
                {
                    new() { Type = CallArgType.Integer, Value = 0L },
                },
            },
            new() { Type = CallArgType.String, Value = "Test attachment" },
        };

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, callArgs);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }

    [Fact]
    public void AssetIdxSizeMismatch_ThrowException()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
        var recipient = PrivateNode.GenerateAccount(0);

        var payments = new List<Amount>
        {
            new() { Value = 10_00000000, },
        };

        var callArgs = new List<CallArg>
        {
            new()
            {
                Type = CallArgType.List, Value = new List<CallArg>
                {
                    new() { Type = CallArgType.ByteArray, Value = new Base64s(Base58s.Decode(recipient.GetAddress().Encoded)) },
                },
            },
            new()
            {
                Type = CallArgType.List, Value = new List<CallArg>
                {
                    new() { Type = CallArgType.Integer, Value = 10_00000000L },
                },
            },
            new()
            {
                Type = CallArgType.List, Value = new List<CallArg>
                {
                    new() { Type = CallArgType.Integer, Value = 0L },
                    new() { Type = CallArgType.Integer, Value = 0L },
                },
            },
            new() { Type = CallArgType.String, Value = "Test attachment" },
        };

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, callArgs);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }

    [Fact]
    public void EmptyArguments_ThrowException()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);

        var payments = new List<Amount>
        {
            new() { Value = 10_00000000, },
        };
        var massTransferItems = new List<MassTransferItem>();

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }

    [Fact]
    public void EmptyPayments_ThrowException()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
        var recipient = PrivateNode.GenerateAccount(0);

        var payments = new List<Amount>();
        var massTransferItems = new List<MassTransferItem>
        {
            new() { Recipient = recipient.GetAddress().Encoded, Amount = 10_00000000, AssetIdx = 0, },
        };

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }

    [Fact]
    public void OutOfRecipientsLimit_ThrowException()
    {
        var sender = PrivateNode.GenerateAccount(102_00000000);
        var recipients = Enumerable.Range(0, 101).Select(_ => PrivateNode.GenerateAccount(0)).ToList();

        var payments = new List<Amount>
        {
            new() { Value = 101_00000000, },
        };
        var massTransferItems = recipients.Select(x => new MassTransferItem
        {
            Recipient = x.GetAddress().Encoded, Amount = 1_00000000, AssetIdx = 0,
        }).ToList();

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData(0L)]
    public void NonPositiveAmount_ThrowException(long amount)
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
            new() { Recipient = recipient2.GetAddress().Encoded, Amount = amount, AssetIdx = 0, },
        };

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData(2L)]
    public void WrongAssetIdx_ThrowException(long assetIdx)
    {
        var sender = PrivateNode.GenerateAccount(101_00000000);
        var assetId = PrivateNode.IssueAsset(sender, "TOKEN", 10_000000, 6);
        var recipient = PrivateNode.GenerateAccount(0);

        var payments = new List<Amount>
        {
            new() { Value = 50_00000000, },
            new() { Value = 8_000000, AssetId = assetId, },
        };
        var massTransferItems = new List<MassTransferItem>
        {
            new() { Recipient = recipient.GetAddress().Encoded, Amount = 10_00000000, AssetIdx = 0, },
            new() { Recipient = recipient.GetAddress().Encoded, Amount = 40_00000000, AssetIdx = assetIdx, },
        };

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }

    [Fact]
    public void InvalidRecipientAddress_ThrowException()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);

        var payments = new List<Amount>
        {
            new() { Value = 50_00000000, },
        };
        var massTransferItems = new List<MassTransferItem>
        {
            new() { Recipient = "QWERTY", Amount = 50_00000000, AssetIdx = 0, },
        };

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        invoke.Should().Throw<Exception>().WithMessage("*Wrong addressBytes length: expected: 26, actual: 5");
    }

    [Fact]
    public void AmountsLessThanPayments_ThrowException()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
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
            new() { Recipient = recipient2.GetAddress().Encoded, Amount = 30_00000000, AssetIdx = 0, },
            new() { Recipient = recipient1.GetAddress().Encoded, Amount = 3_000000, AssetIdx = 1, },
            new() { Recipient = recipient2.GetAddress().Encoded, Amount = 4_000000, AssetIdx = 1, },
        };

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }

    [Fact]
    public void AmountsLargerThanPayments_ThrowException()
    {
        var sender = PrivateNode.GenerateAccount(100_00000000);
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
            new() { Recipient = recipient1.GetAddress().Encoded, Amount = 30_00000000, AssetIdx = 0, },
            new() { Recipient = recipient2.GetAddress().Encoded, Amount = 30_00000000, AssetIdx = 0, },
            new() { Recipient = recipient1.GetAddress().Encoded, Amount = 5_000000, AssetIdx = 1, },
            new() { Recipient = recipient2.GetAddress().Encoded, Amount = 4_000000, AssetIdx = 1, },
        };

        var invoke = () => _massTokenTransferAccount.InvokeMassTransfer(sender, payments, massTransferItems);

        invoke.Should().Throw<Exception>().WithMessage("*Invalid arguments");
    }
}