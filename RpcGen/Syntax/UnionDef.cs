namespace RpcGen;

public class UnionDef : IDefinition
{
    public UnionDef(
        IdentifierToken identifier,
        Declaration condition,
        IList<CaseSpec> cases,
        Declaration? defaultValue)
    {
        this.Identifier = identifier;
        this.Condition = condition;
        this.Cases = cases;
        this.DefaultValue = defaultValue;
    }

    public IList<CaseSpec> Cases { get; }

    public Declaration Condition { get; }

    public Declaration? DefaultValue { get; }

    public IdentifierToken Identifier { get; }

    public static UnionDef Take(TokenReader reader)
    {
        var identifier = reader.ExpectIdentifier();

        reader.ExpectSwitch();

        reader.ExpectParenStart();

        var condition = Declaration.Take(reader);

        reader.ExpectParenEnd();

        reader.ExpectBraceStart();

        var cases = new List<CaseSpec>();
        while (reader.IsCase())
        {
            var c = CaseSpec.Take(reader);
            cases.Add(c);
        }

        Declaration? defaultValue = null;
        if (reader.TryExpectDefault(out var _))
        {
            reader.ExpectColon();

            defaultValue = Declaration.Take(reader);

            reader.ExpectSemicolon();
        }

        reader.ExpectBraceEnd();

        reader.ExpectSemicolon();

        return new UnionDef(identifier, condition, cases, defaultValue);
    }
}
