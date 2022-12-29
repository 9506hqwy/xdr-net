namespace Xdr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class XdrUnionCaseAttribute : XdrUnionArmAttribute
{
    public XdrUnionCaseAttribute(object value)
    {
        this.Value = value;
    }

    public object Value { get; }
}
