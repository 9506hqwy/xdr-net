namespace Xdr.Test;

[TestClass]
public class XdrSerializerTest
{
    private enum EnumTest
    {
        A = 2,
        B = 5,
    }

    [TestMethod]
    public void SerializeBool()
    {
        var v1 = XdrSerializer.Serialize(false);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize(true);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x01 }, v2);
    }

    [TestMethod]
    public void SerializeShort()
    {
        var v1 = XdrSerializer.Serialize((short)0);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize(short.MinValue);
        CollectionAssert.AreEqual(new byte[] { 0xFF, 0xFF, 0x80, 0x00 }, v2);

        var v3 = XdrSerializer.Serialize(short.MaxValue);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x7F, 0xFF }, v3);
    }

    [TestMethod]
    public void SerializeInt()
    {
        var v1 = XdrSerializer.Serialize((int)0);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize(int.MinValue);
        CollectionAssert.AreEqual(new byte[] { 0x80, 0x00, 0x00, 0x00 }, v2);

        var v3 = XdrSerializer.Serialize(int.MaxValue);
        CollectionAssert.AreEqual(new byte[] { 0x7F, 0xFF, 0xFF, 0xFF }, v3);
    }

    [TestMethod]
    public void SerializeLong()
    {
        var v1 = XdrSerializer.Serialize(0L);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize(long.MinValue);
        CollectionAssert.AreEqual(new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, v2);

        var v3 = XdrSerializer.Serialize(long.MaxValue);
        CollectionAssert.AreEqual(new byte[] { 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, v3);
    }

    [TestMethod]
    public void SerializeUshort()
    {
        var v1 = XdrSerializer.Serialize((ushort)0);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize(ushort.MaxValue);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0xFF, 0xFF }, v2);
    }

    [TestMethod]
    public void SerializeUint()
    {
        var v1 = XdrSerializer.Serialize(0U);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize(uint.MaxValue);
        CollectionAssert.AreEqual(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, v2);
    }

    [TestMethod]
    public void SerializeUlong()
    {
        var v1 = XdrSerializer.Serialize(0UL);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize(ulong.MaxValue);
        CollectionAssert.AreEqual(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, v2);
    }

    [TestMethod]
    public void SerializeFloat()
    {
        var v1 = XdrSerializer.Serialize(0.0F);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize(12.5F);
        CollectionAssert.AreEqual(new byte[] { 0x41, 0x48, 0x00, 0x00 }, v2);
    }

    [TestMethod]
    public void SerializeDouble()
    {
        var v1 = XdrSerializer.Serialize(0.0D);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize(12.5D);
        CollectionAssert.AreEqual(new byte[] { 0x40, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, v2);
    }

    [TestMethod]
    public void SerializeString()
    {
        var v1 = XdrSerializer.Serialize("a");
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x61, 0x00, 0x00, 0x00 }, v1);

        var v2 = XdrSerializer.Serialize("あ");
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x03, 0xE3, 0x81, 0x82, 0x00 }, v2);

        var v3 = XdrSerializer.Serialize("abcd");
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x04, 0x61, 0x62, 0x63, 0x64 }, v3);

        var v4 = XdrSerializer.Serialize(string.Empty);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, v4);
    }

    [TestMethod]
    public void SerializeFixedOpaque()
    {
        var v = XdrSerializer.Serialize(new byte[] { 1, 2 });
        CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x00, 0x00 }, v);
    }

    [TestMethod]
    public void SerializeFixedXdrOption()
    {
        var v = XdrSerializer.Serialize(new XdrOption<int>?[] { null, new XdrOption<int>(2) });
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, v);
    }

    [TestMethod]
    public void SerializeVariableOpaque()
    {
        var v = XdrSerializer.Serialize(new List<byte> { 1, 2 });
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x02, 0x01, 0x02, 0x00, 0x00 }, v);
    }

    [TestMethod]
    public void SerializeVariableXdrOption()
    {
        var v = XdrSerializer.Serialize(new List<XdrOption<int>?> { null, new XdrOption<int>(2) });
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, v);
    }

    [TestMethod]
    public void SerializeFixedArray()
    {
        var v = XdrSerializer.Serialize(new int[] { 1, 2 });
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, v);
    }

    [TestMethod]
    public void SerializeVariableArray()
    {
        var v = XdrSerializer.Serialize(new List<int> { 1, 2 });
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, v);
    }

    [TestMethod]
    public void SerializeOptional()
    {
        XdrOption<int>? obj1 = null;
        var v1 = XdrSerializer.Serialize(obj1);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, v1);

        XdrOption<int> obj2 = new XdrOption<int>(2);
        var v2 = XdrSerializer.Serialize(obj2);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, v2);
    }

    [TestMethod]
    public void SerializeVoid()
    {
        var v = XdrSerializer.Serialize(XdrVoid.Data);
        CollectionAssert.AreEqual(new byte[] { }, v);
    }

    [TestMethod]
    public void SerializeEnum()
    {
        var v1 = XdrSerializer.Serialize(EnumTest.A);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x02 }, v1);

        var v2 = XdrSerializer.Serialize(EnumTest.B);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x05 }, v2);
    }

    [TestMethod]
    public void SerializeUnion()
    {
        var obj1 = new UnionTest(1)
        {
            A = new XdrOption<int>(2),
        };
        var v1 = XdrSerializer.Serialize(obj1);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, v1);

        var obj2 = new UnionTest(2)
        {
            B = new XdrOption<long>(2),
        };
        var v2 = XdrSerializer.Serialize(obj2);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 }, v2);

        var obj3 = new UnionTest(3)
        {
            C = new XdrOption<XdrVoid>(XdrVoid.Data),
        };
        var v3 = XdrSerializer.Serialize(obj3);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x03 }, v3);

        var obj4 = new UnionTest(4)
        {
            B = new XdrOption<long>(3),
        };
        var v4 = XdrSerializer.Serialize(obj4);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03 }, v4);
    }

    [TestMethod]
    public void SerializeStruct()
    {
        var obj = new StructTest
        {
            A = 1,
            B = 2,
        };
        var v = XdrSerializer.Serialize(obj);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 }, v);
    }

    [TestMethod]
    public void SerializeStructFixedArray()
    {
        var obj = new StructFixedArrayTest
        {
            A = new[] { 1, 2 },
        };
        var v = XdrSerializer.Serialize(obj);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, v);
    }

    [TestMethod]
    public void SerializeStructVariableArray()
    {
        var obj = new StructVariableArrayTest
        {
            A = { 1, 2 },
        };
        var v = XdrSerializer.Serialize(obj);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, v);
    }

    [TestMethod]
    public void SerializeStructXdrOption()
    {
        var obj1 = new StructXdrOptionTest
        {
            A = null,
        };
        var v1 = XdrSerializer.Serialize(obj1);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, v1);

        var obj2 = new StructXdrOptionTest
        {
            A = new XdrOption<int>(2),
        };
        var v2 = XdrSerializer.Serialize(obj2);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 }, v2);
    }

    private class UnionTest : XdrUnion<int>
    {
        public UnionTest(int value)
            : base(value)
        {
        }

        [XdrUnionCase(1)]
        public XdrOption<int>? A { get; set; }

        [XdrUnionCase(2)]
        [XdrUnionCase(4)]
        public XdrOption<long>? B { get; set; }

        [XdrUnionDefault]
        public XdrOption<XdrVoid>? C { get; set; }
    }

    [XdrStruct]
    private class StructTest
    {
        [XdrElementOrder(1)]
        public int A { get; set; }

        [XdrElementOrder(2)]
        public long B { get; set; }
    }

    [XdrStruct]
    private class StructFixedArrayTest
    {
        [XdrElementOrder(1)]
        [XdrFixedLength(2)]
        public int[] A { get; set; } = Array.Empty<int>();
    }

    [XdrStruct]
    private class StructVariableArrayTest
    {
        [XdrElementOrder(1)]
        public List<int> A { get; set; } = new List<int>();
    }

    [XdrStruct]
    private class StructXdrOptionTest
    {
        [XdrElementOrder(1)]
        public XdrOption<int>? A { get; set; }
    }
}
