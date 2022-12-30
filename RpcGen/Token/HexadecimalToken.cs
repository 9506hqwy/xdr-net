namespace RpcGen;

public sealed class HexadecimalToken : NumberToken
{
    public override object GetValue<T>()
    {
        var t = typeof(T);
        return t switch
        {
            _ when typeof(int) == t => Convert.ToInt32(this.Value, 16),
            _ when typeof(uint) == t => Convert.ToUInt32(this.Value, 16),
            _ when typeof(long) == t => Convert.ToInt64(this.Value, 16),
            _ when typeof(ulong) == t => Convert.ToUInt64(this.Value, 16),
            _ => throw new Exception($"Not supproted number `{this.Value}` ({this.Position})"),
        };
    }
}
