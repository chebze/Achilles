namespace Achilles.Habbo.Messaging;

[AttributeUsage(
    AttributeTargets.Class,
    AllowMultiple = false,
    Inherited = false
)]
public class IncomingMessageAttribute : Attribute
{
    public int Header { get; }

    public IncomingMessageAttribute(int header)
    {
        this.Header = header;
    }
}
