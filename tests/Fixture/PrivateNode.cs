namespace MassTokenTransfer.Tests.Fixture;

public static class PrivateNode
{
    private static readonly PrivateKey MainAccount;

    public const byte ChainId = (byte)'R';
    public static readonly Node Instance;

    static PrivateNode()
    {
        MainAccount = PrivateKey.FromSeed("waves private node seed with waves tokens");
        Instance = new Node("http://localhost:6869");
        try
        {
            Instance.GetHeight();
        }
        catch (Exception)
        {
            throw new Exception("Run the following docker image: https://github.com/wavesplatform/Waves/tree/HEAD/docker");
        }
    }

    public static PrivateKey GenerateAccount(long wavesBalance)
    {
        var seed = Crypto.GenerateRandomSeedPhrase();
        var privateKey = PrivateKey.FromSeed(seed);

        if (wavesBalance > 0)
        {
            TransferTransactionBuilder
                .Params(privateKey.GetAddress(), wavesBalance)
                .GetSignedWith(MainAccount)
                .BroadcastAndWait(Instance);
        }

        return privateKey;
    }

    public static AssetId IssueAsset(PrivateKey issuer, string name, long amount, int decimals) => AssetId.As(IssueTransactionBuilder
        .Params(name, amount, decimals, null, false)
        .GetSignedWith(issuer)
        .BroadcastAndWait(Instance)
        .Transaction.Id!.Encoded);

    public static Address GetAddress(this PrivateKey privateKey) => Address.FromPublicKey(ChainId, privateKey.PublicKey);

    public static TransactionInfo BroadcastAndWait(this Transaction transaction, Node node) => node.WaitForTransaction(node.Broadcast(transaction).Id);
}