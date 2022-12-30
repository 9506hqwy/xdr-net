namespace RpcGen.Test;

[TestClass]
public class OctalTokenTest
{
    [TestMethod]
    public void GetValueInt32()
    {
        var number = new OctalToken
        {
            Value = "010",
        };
        var value = number.GetValue<int>();
        Assert.AreEqual(8, value);
    }

    [TestMethod]
    public void GetValueUInt32()
    {
        var number = new OctalToken
        {
            Value = "010",
        };
        var value = number.GetValue<uint>();
        Assert.AreEqual(8u, value);
    }

    [TestMethod]
    public void GetValueInt64()
    {
        var number = new OctalToken
        {
            Value = "010",
        };
        var value = number.GetValue<long>();
        Assert.AreEqual(8L, value);
    }

    [TestMethod]
    public void GetValueUInt64()
    {
        var number = new OctalToken
        {
            Value = "010",
        };
        var value = number.GetValue<ulong>();
        Assert.AreEqual(8ul, value);
    }
}
