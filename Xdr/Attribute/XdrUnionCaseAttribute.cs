namespace Xdr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class XdrUnionCaseAttribute(object value) : XdrUnionArmAttribute
{
    public object Value { get; } = value;
}
