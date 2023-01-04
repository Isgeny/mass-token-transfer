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
            .GetSignedWith(PrivateKey)
            .BroadcastAndWait(PrivateNode.Instance);
    }

    public PrivateKey PrivateKey { get; }

    public Address Address => PrivateKey.GetAddress();

    public string InvokeMassTransfer(PrivateKey callerAccount, long wavesAmount, List<MassTransferItem> items) => InvokeScriptTransactionBuilder
        .Params(Address, new List<Amount> { new() { Value = wavesAmount } }, new Call
        {
            Function = "massTransfer", Args = new List<CallArg>
            {
                new() { Type = CallArgType.List, Value = items.Select(x => new CallArg { Type = CallArgType.String, Value = x.Recipient }).ToList() },
                new() { Type = CallArgType.List, Value = items.Select(x => new CallArg { Type = CallArgType.Integer, Value = x.Amount }).ToList() },
                new() { Type = CallArgType.List, Value = items.Select(x => new CallArg { Type = CallArgType.String, Value = x.Asset }).ToList() },
            }
        })
        .GetSignedWith(callerAccount)
        .BroadcastAndWait(PrivateNode.Instance)
        .Transaction.Id!;
}