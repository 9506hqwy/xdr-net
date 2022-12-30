namespace RpcGen.Test;

[TestClass]
public class ProtoReaderTest
{
    [TestMethod]
    public void Read()
    {
        using var mem = new MemoryStream(new byte[] { 0x01, 0x02 });
        using var reader = new ProtoReader(mem);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(1, reader.CurrentColumn);

        Assert.AreEqual(1, reader.Peek());
        Assert.IsFalse(reader.EndOfStream);

        var b = reader.Read();
        Assert.AreEqual(1, b);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(2, reader.CurrentColumn);

        Assert.AreEqual(2, reader.Peek());
        Assert.IsFalse(reader.EndOfStream);

        b = reader.Read();
        Assert.AreEqual(2, b);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(3, reader.CurrentColumn);

        Assert.AreEqual(-1, reader.Peek());
        Assert.IsFalse(reader.EndOfStream);

        b = reader.Read();
        Assert.AreEqual(-1, b);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(3, reader.CurrentColumn);

        Assert.AreEqual(-1, reader.Peek());
        Assert.IsTrue(reader.EndOfStream);
    }

    [TestMethod]
    public void Skip()
    {
        using var mem = new MemoryStream(new byte[] { 0x01, 0x02, 0x03, 0x04 });
        using var reader = new ProtoReader(mem);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(1, reader.CurrentColumn);

        Assert.AreEqual(1, reader.Peek());
        Assert.IsFalse(reader.EndOfStream);

        reader.Skip(1);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(2, reader.CurrentColumn);

        Assert.AreEqual(2, reader.Peek());
        Assert.IsFalse(reader.EndOfStream);

        reader.Skip(2);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(4, reader.CurrentColumn);

        Assert.AreEqual(4, reader.Peek());
        Assert.IsFalse(reader.EndOfStream);

        reader.Skip(1);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(5, reader.CurrentColumn);

        Assert.AreEqual(-1, reader.Peek());
        Assert.IsFalse(reader.EndOfStream);

        reader.Skip(1);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(5, reader.CurrentColumn);

        Assert.AreEqual(-1, reader.Peek());
        Assert.IsTrue(reader.EndOfStream);
    }

    [TestMethod]
    public void SkipWhileAny()
    {
        using var mem = new MemoryStream(new byte[] { 0x01, 0x02, 0x03, 0x04 });
        using var reader = new ProtoReader(mem);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(1, reader.CurrentColumn);

        reader.SkipWhileAny(new int[] { 0x01 });
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(2, reader.CurrentColumn);

        Assert.AreEqual(2, reader.Peek());

        reader.SkipWhileAny(new int[] { 0x02, 0x03 });
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(4, reader.CurrentColumn);

        Assert.AreEqual(4, reader.Peek());

        reader.SkipWhileAny(new int[] { 0x04 });
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(5, reader.CurrentColumn);

        Assert.AreEqual(-1, reader.Peek());
        Assert.IsFalse(reader.EndOfStream);

        reader.Skip(1);
        Assert.AreEqual(1, reader.CurrentLine);
        Assert.AreEqual(5, reader.CurrentColumn);

        Assert.AreEqual(-1, reader.Peek());
        Assert.IsTrue(reader.EndOfStream);
    }
}
