namespace Xdr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class XdrFixedLengthAttribute(int length) : Attribute
{
    public int Length { get; } = length;
}
