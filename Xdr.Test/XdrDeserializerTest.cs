namespace Xdr.Test;

#pragma warning disable CA1812
[TestClass]
public class XdrDeserializerTest
{
    private static readonly int[] FixedArray = [1, 2];

    private enum EnumTest
    {
        A = 2,
        B = 5,
    }

    [TestMethod]
    public void DeserializeBool()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<bool>(bytes1, out var rest1);
        Assert.IsFalse((bool?)v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x00, 0x00, 0x00, 0x01 };
        var v2 = XdrDeserializer.Deserialize<bool>(bytes2, out var rest2);
        Assert.IsTrue((bool?)v2);
        Assert.IsFalse(rest2.Any());
    }

    [TestMethod]
    public void DeserializeByte()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<byte>(bytes1, out var rest1);
        Assert.AreEqual((byte)0x00, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x00, 0x00, 0x00, 0x01 };
        var v2 = XdrDeserializer.Deserialize<byte>(bytes2, out var rest2);
        Assert.AreEqual((byte)0x01, v2);
        Assert.IsFalse(rest2.Any());

        var bytes3 = new byte[] { 0x00, 0x00, 0x00, 0xFF };
        var v3 = XdrDeserializer.Deserialize<byte>(bytes3, out var rest3);
        Assert.AreEqual((byte)0xFF, v3);
        Assert.IsFalse(rest3.Any());
    }

    [TestMethod]
    public void DeserializeChar()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x61 };
        var v1 = XdrDeserializer.Deserialize<char>(bytes1, out var rest1);
        Assert.AreEqual('a', v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x00, 0x00, 0x30, 0x42 };
        var v2 = XdrDeserializer.Deserialize<char>(bytes2, out var rest2);
        Assert.AreEqual('あ', v2);
        Assert.IsFalse(rest2.Any());
    }

    [TestMethod]
    public void DeserializeShort()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<short>(bytes1, out var rest1);
        Assert.AreEqual((short)0, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0xFF, 0xFF, 0x80, 0x0 };
        var v2 = XdrDeserializer.Deserialize<short>(bytes2, out var rest2);
        Assert.AreEqual(short.MinValue, v2);
        Assert.IsFalse(rest2.Any());

        var bytes3 = new byte[] { 0x00, 0x00, 0x7F, 0xFF };
        var v3 = XdrDeserializer.Deserialize<short>(bytes3, out var rest3);
        Assert.AreEqual(short.MaxValue, v3);
        Assert.IsFalse(rest3.Any());
    }

    [TestMethod]
    public void DeserializeInt()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<int>(bytes1, out var rest1);
        Assert.AreEqual(0, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x80, 0x00, 0x00, 0x00 };
        var v2 = XdrDeserializer.Deserialize<int>(bytes2, out var rest2);
        Assert.AreEqual(int.MinValue, v2);
        Assert.IsFalse(rest2.Any());

        var bytes3 = new byte[] { 0x7F, 0xFF, 0xFF, 0xFF };
        var v3 = XdrDeserializer.Deserialize<int>(bytes3, out var rest3);
        Assert.AreEqual(int.MaxValue, v3);
        Assert.IsFalse(rest3.Any());
    }

    [TestMethod]
    public void DeserializeLong()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<long>(bytes1, out var rest1);
        Assert.AreEqual(0L, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var v2 = XdrDeserializer.Deserialize<long>(bytes2, out var rest2);
        Assert.AreEqual(long.MinValue, v2);
        Assert.IsFalse(rest2.Any());

        var bytes3 = new byte[] { 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        var v3 = XdrDeserializer.Deserialize<long>(bytes3, out var rest3);
        Assert.AreEqual(long.MaxValue, v3);
        Assert.IsFalse(rest3.Any());
    }

    [TestMethod]
    public void DeserializeUshort()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<ushort>(bytes1, out var rest1);
        Assert.AreEqual((ushort)0, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x00, 0x00, 0xFF, 0xFF };
        var v2 = XdrDeserializer.Deserialize<ushort>(bytes2, out var rest2);
        Assert.AreEqual(ushort.MaxValue, v2);
        Assert.IsFalse(rest2.Any());
    }

    [TestMethod]
    public void DeserializeUint()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<uint>(bytes1, out var rest1);
        Assert.AreEqual(0U, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
        var v2 = XdrDeserializer.Deserialize<uint>(bytes2, out var rest2);
        Assert.AreEqual(uint.MaxValue, v2);
        Assert.IsFalse(rest2.Any());
    }

    [TestMethod]
    public void DeserializeUlong()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<ulong>(bytes1, out var rest1);
        Assert.AreEqual(0UL, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        var v2 = XdrDeserializer.Deserialize<ulong>(bytes2, out var rest2);
        Assert.AreEqual(ulong.MaxValue, v2);
        Assert.IsFalse(rest2.Any());
    }

    [TestMethod]
    public void DeserializeFloat()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<float>(bytes1, out var rest1);
        Assert.AreEqual(0.0F, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x41, 0x48, 0x00, 0x00 };
        var v2 = XdrDeserializer.Deserialize<float>(bytes2, out var rest2);
        Assert.AreEqual(12.5F, v2);
        Assert.IsFalse(rest2.Any());
    }

    [TestMethod]
    public void DeserializeDouble()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<double>(bytes1, out var rest1);
        Assert.AreEqual(0.0D, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x40, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var v2 = XdrDeserializer.Deserialize<double>(bytes2, out var rest2);
        Assert.AreEqual(12.5D, v2);
        Assert.IsFalse(rest2.Any());
    }

    [TestMethod]
    public void DeserializeString()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x61, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<string>(bytes1, out var rest1);
        Assert.AreEqual("a", v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x00, 0x00, 0x00, 0x03, 0xE3, 0x81, 0x82, 0x00 };
        var v2 = XdrDeserializer.Deserialize<string>(bytes2, out var rest2);
        Assert.AreEqual("あ", v2);
        Assert.IsFalse(rest2.Any());

        var bytes3 = new byte[] { 0x00, 0x00, 0x00, 0x04, 0x61, 0x62, 0x63, 0x64 };
        var v3 = XdrDeserializer.Deserialize<string>(bytes3, out var rest3);
        Assert.AreEqual("abcd", v3);
        Assert.IsFalse(rest3.Any());

        var bytes4 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v4 = XdrDeserializer.Deserialize<string>(bytes4, out var rest4);
        Assert.AreEqual(string.Empty, v4);
        Assert.IsFalse(rest4.Any());
    }

    [TestMethod]
    public void DeserializeFixedOpaque()
    {
        var bytes = new byte[] { 0x01, 0x02, 0x00, 0x00 };
        var v = (byte[])XdrDeserializer.Deserialize<byte[]>(bytes, 2, out var rest);
        CollectionAssert.AreEqual(new byte[] { 1, 2 }, v);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeFixedXdrOption()
    {
        var bytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 };
        var v = (XdrOption<int>[])XdrDeserializer.Deserialize<XdrOption<int>?[]>(bytes, 2, out var rest);
        Assert.IsNull(v[0]);
        Assert.AreEqual(2, v[1].Value);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeVariableOpaque()
    {
        var bytes = new byte[] { 0x00, 0x00, 0x00, 0x02, 0x01, 0x02, 0x00, 0x00 };
        var v = (List<byte>)XdrDeserializer.Deserialize<List<byte>>(bytes, out var rest);
        CollectionAssert.AreEqual(new List<byte> { 1, 2 }, v);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeVariableXdrOption()
    {
        var bytes = new byte[] { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 };
        var v = (List<XdrOption<int>>)XdrDeserializer.Deserialize<List<XdrOption<int>?>>(bytes, out var rest);
        Assert.IsNull(v[0]);
        Assert.AreEqual(2, v[1].Value);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeFixedArray()
    {
        var bytes = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 };
        var v = (int[])XdrDeserializer.Deserialize<int[]>(bytes, 2, out var rest);
        CollectionAssert.AreEqual(FixedArray, v);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeVariableArray()
    {
        var bytes = new byte[] { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 };
        var v = (List<int>)XdrDeserializer.Deserialize<List<int>>(bytes, out var rest);
        CollectionAssert.AreEqual(new List<int> { 1, 2 }, v);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeOptional()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v1 = XdrDeserializer.Deserialize<XdrOption<int>>(bytes1, out var rest1);
        Assert.IsNull(v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 };
        var v2 = XdrDeserializer.Deserialize<XdrOption<int>>(bytes2, out var rest2);
        Assert.AreEqual(2, (v2 as XdrOption<int>)!.Value);
        Assert.IsFalse(rest2.Any());
    }

    [TestMethod]
    public void DeserializeVoid()
    {
        var bytes = Array.Empty<byte>();
        var v = XdrDeserializer.Deserialize<XdrVoid>(bytes, out var rest);
        Assert.AreEqual(XdrVoid.Data, v);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeEnum()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x02 };
        var v1 = XdrDeserializer.Deserialize<EnumTest>(bytes1, out var rest1);
        Assert.AreEqual(EnumTest.A, v1);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x00, 0x00, 0x00, 0x05 };
        var v2 = XdrDeserializer.Deserialize<EnumTest>(bytes2, out var rest2);
        Assert.AreEqual(EnumTest.B, v2);
        Assert.IsFalse(rest2.Any());
    }

    [TestMethod]
    public void DeserializeUnion()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 };
        var v1 = (UnionTest)XdrDeserializer.Deserialize<UnionTest>(bytes1, out var rest1);
        Assert.AreEqual(1, v1.Value);
        Assert.AreEqual(2, v1!.A!.Data);
        Assert.IsNull(v1.B);
        Assert.IsNull(v1.C);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 };
        var v2 = (UnionTest)XdrDeserializer.Deserialize<UnionTest>(bytes2, out var rest2);
        Assert.AreEqual(2, v2.Value);
        Assert.IsNull(v2!.A);
        Assert.AreEqual(2L, v2.B!.Data);
        Assert.IsNull(v2.C);
        Assert.IsFalse(rest2.Any());

        var bytes3 = new byte[] { 0x00, 0x00, 0x00, 0x03 };
        var v3 = (UnionTest)XdrDeserializer.Deserialize<UnionTest>(bytes3, out var rest3);
        Assert.AreEqual(3, v3.Value);
        Assert.IsNull(v3!.A);
        Assert.IsNull(v3.B);
        Assert.AreEqual(XdrVoid.Data, v3.C!.Data);
        Assert.IsFalse(rest3.Any());

        var bytes4 = new byte[] { 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03 };
        var v4 = (UnionTest)XdrDeserializer.Deserialize<UnionTest>(bytes4, out var rest4);
        Assert.AreEqual(4, v4.Value);
        Assert.IsNull(v4!.A);
        Assert.AreEqual(3L, v4.B!.Data);
        Assert.IsNull(v4.C);
        Assert.IsFalse(rest4.Any());
    }

    [TestMethod]
    public void DeserializeStruct()
    {
        var bytes = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 };
        var v = (StructTest)XdrDeserializer.Deserialize<StructTest>(bytes, out var rest);
        Assert.AreEqual(1, v.A);
        Assert.AreEqual(2L, v.B);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeStructFixedArray()
    {
        var bytes = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 };
        var v = (StructFixedArrayTest)XdrDeserializer.Deserialize<StructFixedArrayTest>(bytes, out var rest);
        CollectionAssert.AreEqual(FixedArray, v!.A);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeStructVariableArray()
    {
        var bytes = new byte[] { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 };
        var v = (StructVariableArrayTest)XdrDeserializer.Deserialize<StructVariableArrayTest>(bytes, out var rest);
        CollectionAssert.AreEqual(new List<int> { 1, 2 }, v!.A);
        Assert.IsFalse(rest.Any());
    }

    [TestMethod]
    public void DeserializeStructXdrOption()
    {
        var bytes1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var v1 = (StructXdrOptionTest)XdrDeserializer.Deserialize<StructXdrOptionTest>(bytes1, out var rest1);
        Assert.IsNull(v1.A);
        Assert.IsFalse(rest1.Any());

        var bytes2 = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 };
        var v2 = (StructXdrOptionTest)XdrDeserializer.Deserialize<StructXdrOptionTest>(bytes2, out var rest2);
        Assert.AreEqual(2, v2.A!.Value);
        Assert.IsFalse(rest2.Any());
    }

    private class UnionTest(int value) : XdrUnion<int>(value)
    {
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
        public int[] A { get; set; } = [];
    }

    [XdrStruct]
    private class StructVariableArrayTest
    {
        [XdrElementOrder(1)]
        public List<int> A { get; set; } = [];
    }

    [XdrStruct]
    private class StructXdrOptionTest
    {
        [XdrElementOrder(1)]
        public XdrOption<int>? A { get; set; }
    }
}
#pragma warning restore CA1812
