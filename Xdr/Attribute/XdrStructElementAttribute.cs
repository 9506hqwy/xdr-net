namespace Xdr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class XdrElementOrderAttribute(int order) : Attribute
{
    internal int Order { get; } = order;
}
