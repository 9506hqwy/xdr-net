namespace RpcGen;

public class Constant
{
    public Constant(IdentifierToken identifier)
    {
        this.Identifier = identifier;
    }

    public Constant(NumberToken number)
    {
        this.Number = number;
    }

    public IdentifierToken? Identifier { get; }

    public NumberToken? Number { get; }

    public static Constant Take(TokenReader reader)
    {
        if (reader.TryExpectNumber(out var number))
        {
            return new Constant(number!);
        }

        var identfier = reader.ExpectIdentifier();
        return new Constant(identfier);
    }
}
