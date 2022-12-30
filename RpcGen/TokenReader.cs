namespace RpcGen;

public class TokenReader
{
    private readonly IEnumerator<Token> tokens;

    public TokenReader(Lexer lexer)
    {
        this.tokens = lexer
            .Enumerate()
            .Where(t => t is not WhitespaceToken)
            .Where(t => t is not CommentToken)
            .GetEnumerator();
    }

    public Token Current => this.tokens.Current;

    public bool Empty => this.Current is null;

    public bool Next() => this.tokens.MoveNext();

    public SeparatorToken ExpectAngleBracketEnd()
    {
        return this.ExpectSeparator(SeparatorType.AngleBracketEnd);
    }

    public SeparatorToken ExpectAngleBracketStart()
    {
        return this.ExpectSeparator(SeparatorType.AngleBracketStart);
    }

    public SeparatorToken ExpectBraceEnd()
    {
        return this.ExpectSeparator(SeparatorType.BraceEnd);
    }

    public SeparatorToken ExpectBraceStart()
    {
        return this.ExpectSeparator(SeparatorType.BraceStart);
    }

    public SeparatorToken ExpectBracketEnd()
    {
        return this.ExpectSeparator(SeparatorType.BracketEnd);
    }

    public SeparatorToken ExpectBracketStart()
    {
        return this.ExpectSeparator(SeparatorType.BracketStart);
    }

    public ReservedToken ExpectCase()
    {
        return this.ExpectReserved(ReservedType.KeywordCase);
    }

    public SeparatorToken ExpectColon()
    {
        return this.ExpectSeparator(SeparatorType.Colon);
    }

    public SeparatorToken ExpectComma()
    {
        return this.ExpectSeparator(SeparatorType.Comma);
    }

    public SeparatorToken ExpectEqual()
    {
        return this.ExpectSeparator(SeparatorType.Equal);
    }

    public IdentifierToken ExpectIdentifier()
    {
        return this.Expect<IdentifierToken>();
    }

    public NumberToken ExpectNumber()
    {
        return this.Expect<NumberToken>();
    }

    public ReservedToken ExpectReserved()
    {
        return this.Expect<ReservedToken>();
    }

    public SeparatorToken ExpectParenEnd()
    {
        return this.ExpectSeparator(SeparatorType.ParenEnd);
    }

    public SeparatorToken ExpectParenStart()
    {
        return this.ExpectSeparator(SeparatorType.ParenStart);
    }

    public SeparatorToken ExpectSemicolon()
    {
        return this.ExpectSeparator(SeparatorType.Semicolon);
    }

    public ReservedToken ExpectSwitch()
    {
        return this.ExpectReserved(ReservedType.KeywordSwitch);
    }

    public ReservedToken ExpectVersion()
    {
        return this.ExpectReserved(ReservedType.KeywordVersion);
    }

    public bool IsCase()
    {
        return this.IsReserved(ReservedType.KeywordCase);
    }

    public bool TryExpectAngleBracketEnd(out SeparatorToken? token)
    {
        return this.TryExpectSeparator(SeparatorType.AngleBracketEnd, out token);
    }

    public bool TryExpectAngleBracketStart(out SeparatorToken? token)
    {
        return this.TryExpectSeparator(SeparatorType.AngleBracketStart, out token);
    }

    public bool TryExpectBraceEnd(out SeparatorToken? token)
    {
        return this.TryExpectSeparator(SeparatorType.BraceEnd, out token);
    }

    public bool TryExpectBracketStart(out SeparatorToken? token)
    {
        return this.TryExpectSeparator(SeparatorType.BracketStart, out token);
    }

    public bool TryExpectCase(out ReservedToken? token)
    {
        return this.TryExpectReserved(ReservedType.KeywordCase, out token);
    }

    public bool TryExpectDefault(out ReservedToken? token)
    {
        return this.TryExpectReserved(ReservedType.KeywordDefault, out token);
    }

    public bool TryExpectIdentifier(out IdentifierToken? token)
    {
        return this.TryExpect(out token);
    }

    public bool TryExpectNumber(out NumberToken? token)
    {
        return this.TryExpect(out token);
    }

    public bool TryExpectReservedToken(out ReservedToken? token)
    {
        return this.TryExpect(out token);
    }

    private T Expect<T>(Predicate<T>? condition = null)
        where T : Token
    {
        if (this.Current is T token && (condition is null || condition(token)))
        {
            this.Next();
            return token;
        }

        throw new Exception($"Unexpected token {this.Current.Value} ({this.Current.Position}).");
    }

    private SeparatorToken ExpectSeparator(SeparatorType type)
    {
        return this.Expect<SeparatorToken>(t => t.Type == type);
    }

    private ReservedToken ExpectReserved(ReservedType type)
    {
        return this.Expect<ReservedToken>(t => t.Type == type);
    }

    private bool IsReserved(ReservedType type)
    {
        return this.Current is ReservedToken token && token.Type == type;
    }

    private bool TryExpect<T>(out T? token, Predicate<T>? condition = null)
        where T : Token
    {
        if (this.Current is T t && (condition is null || condition(t)))
        {
            token = this.Expect<T>(condition);
            return true;
        }

        token = null;
        return false;
    }

    private bool TryExpectSeparator(SeparatorType type, out SeparatorToken? token)
    {
        return this.TryExpect<SeparatorToken>(out token, t => t.Type == type);
    }

    private bool TryExpectReserved(ReservedType type, out ReservedToken? token)
    {
        return this.TryExpect<ReservedToken>(out token, t => t.Type == type);
    }
}
