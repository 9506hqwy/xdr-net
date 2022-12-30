namespace RpcGen.Test;

[TestClass]
public class DecimalTokenTest
{
    [TestMethod]
    public void GetValueInt32()
    {
        var number = new DecimalToken
        {
            Value = "10",
        };
        var value = number.GetValue<int>();
        Assert.AreEqual(10, value);
    }

    [TestMethod]
    public void GetValueUInt32()
    {
        var number = new DecimalToken
        {
            Value = "10",
        };
        var value = number.GetValue<uint>();
        Assert.AreEqual(10u, value);
    }

    [TestMethod]
    public void GetValueInt64()
    {
        var number = new DecimalToken
        {
            Value = "10",
        };
        var value = number.GetValue<long>();
        Assert.AreEqual(10L, value);
    }

    [TestMethod]
    public void GetValueUInt64()
    {
        var number = new DecimalToken
        {
            Value = "10",
        };
        var value = number.GetValue<ulong>();
        Assert.AreEqual(10ul, value);
    }
}
