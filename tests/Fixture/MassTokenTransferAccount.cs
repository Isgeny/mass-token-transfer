namespace MassTokenTransfer.Tests.Fixture;

public class MassTokenTransferAccount
{
    private const string ScriptPath = "../../../../scripts/mass-token-transfer.ride";

    public MassTokenTransferAccount()
    {
        PrivateKey = PrivateNode.GenerateAccount();

        var scriptText = File.ReadAllText(ScriptPath);

        var scriptInfo = PrivateNode.Instance.CompileScript(scriptText);

        SetScriptTransactionBuilder
            .Params(scriptInfo.Script!)
            .GetSignedWith(PrivateKey)
            .BroadcastAndWait(PrivateNode.Instance);
    }

    public PrivateKey PrivateKey { get; }

    public Address Address => PrivateKey.GetAddress();
}