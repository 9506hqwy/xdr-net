namespace Xdr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class XdrElementOrderAttribute : Attribute
{
    public XdrElementOrderAttribute(int order)
    {
        this.Order = order;
    }

    internal int Order { get; }
}
