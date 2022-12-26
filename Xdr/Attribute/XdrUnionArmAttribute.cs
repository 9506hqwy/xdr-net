namespace Xdr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class XdrUnionArmAttribute : Attribute
{
    public XdrUnionArmAttribute(int value)
    {
        this.Value = value;
    }

    public int Value { get; }
}
