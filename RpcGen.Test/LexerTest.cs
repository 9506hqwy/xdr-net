using System.Text;

namespace RpcGen.Test;

[TestClass]
public class LexerTest
{
    [TestMethod]
    public void EnumerateCommentBlock()
    {
        string content = "/* a */";
        var token = ParseToken(content).Single();
        Assert.IsTrue(token is CommentToken);
        Assert.AreEqual(" a ", token.Value);
    }

    [TestMethod]
    public void EnumerateCommentLine()
    {
        string content = "% a";
        var token = ParseToken(content).Single();
        Assert.IsTrue(token is CommentToken);
        Assert.AreEqual(" a", token.Value);

        content = "% a\n";
        var tokens = ParseToken(content);
        Assert.IsTrue(tokens[0] is CommentToken);
        Assert.IsTrue(tokens[1] is WhitespaceToken);
        Assert.AreEqual(" a", tokens[0].Value);
        Assert.AreEqual("\n", tokens[1].Value);
    }

    [TestMethod]
    public void EnumerateWhitespace()
    {
        string content = " \t\r\n";
        var token = ParseToken(content).Single();
        Assert.IsTrue(token is WhitespaceToken);
    }

    [TestMethod]
    public void EnumerateSeparator()
    {
        string content = "}{][)(><,=;:";
        var tokens = ParseToken(content).OfType<SeparatorToken>().ToArray();
        Assert.AreEqual(SeparatorType.BraceEnd, tokens[0].Type);
        Assert.AreEqual(SeparatorType.BraceStart, tokens[1].Type);
        Assert.AreEqual(SeparatorType.BracketEnd, tokens[2].Type);
        Assert.AreEqual(SeparatorType.BracketStart, tokens[3].Type);
        Assert.AreEqual(SeparatorType.ParenEnd, tokens[4].Type);
        Assert.AreEqual(SeparatorType.ParenStart, tokens[5].Type);
        Assert.AreEqual(SeparatorType.AngleBracketEnd, tokens[6].Type);
        Assert.AreEqual(SeparatorType.AngleBracketStart, tokens[7].Type);
        Assert.AreEqual(SeparatorType.Comma, tokens[8].Type);
        Assert.AreEqual(SeparatorType.Equal, tokens[9].Type);
        Assert.AreEqual(SeparatorType.Semicolon, tokens[10].Type);
        Assert.AreEqual(SeparatorType.Colon, tokens[11].Type);
    }

    [TestMethod]
    public void EnumerateHexadecimal()
    {
        string content = "0x1";
        var token = ParseToken(content).Single();
        Assert.IsTrue(token is HexadecimalToken);
        Assert.AreEqual("0x1", token.Value);
    }

    [TestMethod]
    public void EnumerateOctal()
    {
        string content = "01";
        var token = ParseToken(content).Single();
        Assert.IsTrue(token is OctalToken);
        Assert.AreEqual("01", token.Value);
    }

    [TestMethod]
    public void EnumerateDecimal()
    {
        string content = "0";
        var token = ParseToken(content).Single();
        Assert.IsTrue(token is DecimalToken);
        Assert.AreEqual("0", token.Value);

        content = "1";
        token = ParseToken(content).Single();
        Assert.IsTrue(token is DecimalToken);
        Assert.AreEqual("1", token.Value);

        content = "-1";
        token = ParseToken(content).Single();
        Assert.IsTrue(token is DecimalToken);
        Assert.AreEqual("-1", token.Value);
    }

    [TestMethod]
    public void EnumeratIdentifier()
    {
        string content = "a";
        var token = ParseToken(content).Single();
        Assert.IsTrue(token is IdentifierToken);
        Assert.AreEqual("a", token.Value);
    }

    [TestMethod]
    public void EnumeratUnknown()
    {
        string content = "0a";
        var token = ParseToken(content).Single();
        Assert.IsTrue(token is UnknownToken);
        Assert.AreEqual("0a", token.Value);
    }

    private static Token[] ParseToken(string content)
    {
        using var mem = CreateStream(content);
        using var reader = new ProtoReader(mem);
        var lexer = new Lexer(reader);
        return [.. lexer.Enumerate()];
    }

    private static MemoryStream CreateStream(string conent)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(conent));
    }
}
