namespace RpcGen;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class KeywordValueAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}
