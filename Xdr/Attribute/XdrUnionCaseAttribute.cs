namespace Xdr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class XdrUnionCaseAttribute : XdrUnionArmAttribute
{
    public XdrUnionCaseAttribute(int value)
        : base(value)
    {
    }
}
