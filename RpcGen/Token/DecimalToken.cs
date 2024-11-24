namespace RpcGen;

using System.Globalization;

public sealed class DecimalToken : NumberToken
{
    public override object GetValue<T>()
    {
        var t = typeof(T);
        return t switch
        {
            _ when typeof(int) == t => Convert.ToInt32(this.Value, CultureInfo.CurrentCulture),
            _ when typeof(uint) == t => Convert.ToUInt32(this.Value, CultureInfo.CurrentCulture),
            _ when typeof(long) == t => Convert.ToInt64(this.Value, CultureInfo.CurrentCulture),
            _ when typeof(ulong) == t => Convert.ToUInt64(this.Value, CultureInfo.CurrentCulture),
            _ => throw new Exception($"Not supproted number `{this.Value}` ({this.Position})"),
        };
    }
}
