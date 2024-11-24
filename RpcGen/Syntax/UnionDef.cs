namespace RpcGen;

public class UnionDef(
    IdentifierToken identifier,
    Declaration condition,
    IList<CaseSpec> cases,
    Declaration? defaultValue) : IDefinition
{
    public IList<CaseSpec> Cases { get; } = cases;

    public Declaration Condition { get; } = condition;

    public Declaration? DefaultValue { get; } = defaultValue;

    public IdentifierToken Identifier { get; } = identifier;

    public static UnionDef Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var identifier = reader.ExpectIdentifier();

        _ = reader.ExpectSwitch();

        _ = reader.ExpectParenStart();

        var condition = Declaration.Take(reader);

        _ = reader.ExpectParenEnd();

        _ = reader.ExpectBraceStart();

        var cases = new List<CaseSpec>();
        while (reader.IsCase())
        {
            var c = CaseSpec.Take(reader);
            cases.Add(c);
        }

        Declaration? defaultValue = null;
        if (reader.TryExpectDefault(out _))
        {
            _ = reader.ExpectColon();

            defaultValue = Declaration.Take(reader);

            _ = reader.ExpectSemicolon();
        }

        _ = reader.ExpectBraceEnd();

        _ = reader.ExpectSemicolon();

        return new UnionDef(identifier, condition, cases, defaultValue);
    }
}
