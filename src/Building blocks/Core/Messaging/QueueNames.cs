namespace NSE.Core.Messaging;

public class QueueNames
{
    public const string RegisterCustomer = "RegisterCustomer";
    public static string[] All => new[]
    {
        RegisterCustomer,
    };
}
