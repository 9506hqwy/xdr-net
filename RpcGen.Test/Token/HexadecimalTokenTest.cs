namespace RpcGen.Test;

[TestClass]
public class HexadecimalTokenTest
{
    [TestMethod]
    public void GetValueInt32()
    {
        var number = new HexadecimalToken
        {
            Value = "0x10",
        };
        var value = number.GetValue<int>();
        Assert.AreEqual(16, value);
    }

    [TestMethod]
    public void GetValueUInt32()
    {
        var number = new HexadecimalToken
        {
            Value = "0x10",
        };
        var value = number.GetValue<uint>();
        Assert.AreEqual(16u, value);
    }

    [TestMethod]
    public void GetValueInt64()
    {
        var number = new HexadecimalToken
        {
            Value = "0x10",
        };
        var value = number.GetValue<long>();
        Assert.AreEqual(16L, value);
    }

    [TestMethod]
    public void GetValueUInt64()
    {
        var number = new HexadecimalToken
        {
            Value = "0x10",
        };
        var value = number.GetValue<ulong>();
        Assert.AreEqual(16ul, value);
    }
}
