namespace MassTokenTransfer.Tests.Fixture;

public class MassTokenTransferAccount
{
    private const string ScriptPath = "../../../../scripts/mass-token-transfer.ride";

    public MassTokenTransferAccount()
    {
        PrivateKey = PrivateNode.GenerateAccount(1_00000000);

        var scriptText = File.ReadAllText(ScriptPath);

        var scriptInfo = PrivateNode.Instance.CompileScript(scriptText);

        SetScriptTransactionBuilder
            .Params(scriptInfo.Script!)
            .SetFee(100000000)
            .GetSignedWith(PrivateKey)
            .BroadcastAndWait(PrivateNode.Instance);
    }

    public PrivateKey PrivateKey { get; }

    public Address Address => PrivateKey.GetAddress();

    public string InvokeMassTransfer(PrivateKey callerAccount, ICollection<Amount> payments, ICollection<MassTransferItem> items) => InvokeMassTransfer(callerAccount, payments, new List<CallArg>
    {
        new() { Type = CallArgType.List, Value = items.Select(x => new CallArg { Type = CallArgType.ByteArray, Value = new Base64s(Base58s.Decode(x.Recipient)) }).ToList() },
        new() { Type = CallArgType.List, Value = items.Select(x => new CallArg { Type = CallArgType.Integer, Value = x.Amount }).ToList() },
        new() { Type = CallArgType.List, Value = items.Select(x => new CallArg { Type = CallArgType.Integer, Value = x.AssetIdx }).ToList() },
    });

    public string InvokeMassTransfer(PrivateKey callerAccount, ICollection<Amount> payments, ICollection<CallArg> callArgs) => InvokeScriptTransactionBuilder
        .Params(Address, payments, new Call { Function = "massTransfer", Args = callArgs })
        .GetSignedWith(callerAccount)
        .BroadcastAndWait(PrivateNode.Instance)
        .Transaction.Id!;
}