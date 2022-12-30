namespace RpcGen;

public class Parser
{
    private readonly TokenReader reader;

    public Parser(Lexer lexer)
    {
        this.reader = new TokenReader(lexer);
    }

    public Specification Parse()
    {
        return Specification.Take(this.reader);
    }
}
