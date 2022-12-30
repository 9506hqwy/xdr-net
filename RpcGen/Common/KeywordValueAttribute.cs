namespace RpcGen;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class KeywordValueAttribute : Attribute
{
    public KeywordValueAttribute(string value)
    {
        this.Value = value;
    }

    public string Value { get; }
}
