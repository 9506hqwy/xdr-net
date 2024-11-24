namespace RpcGen;

public class Parser(Lexer lexer)
{
    private readonly TokenReader reader = new(lexer);

    public Specification Parse()
    {
        return Specification.Take(this.reader);
    }
}
