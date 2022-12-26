namespace Xdr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class XdrUnionDefaultAttribute : XdrUnionArmAttribute
{
    public XdrUnionDefaultAttribute(int value)
        : base(value)
    {
    }
}
