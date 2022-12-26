namespace Xdr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class XdrFixedLengthAttribute : Attribute
{
    public XdrFixedLengthAttribute(int length)
    {
        this.Length = length;
    }

    public int Length { get; }
}
