namespace RpcGen;

public class ProgramDef : IDefinition
{
    public ProgramDef(IdentifierToken identifier, IList<VersionDef> versions, Value value)
    {
        this.Identifier = identifier;
        this.Versions = versions;
        this.Value = value;
    }

    public IdentifierToken Identifier { get; }

    public Value Value { get; }

    public IList<VersionDef> Versions { get; }

    public static ProgramDef Take(TokenReader reader)
    {
        var identifier = reader.ExpectIdentifier();

        reader.ExpectBraceStart();

        var versions = new List<VersionDef>();
        while (!reader.Empty)
        {
            if (reader.TryExpectBraceEnd(out var _))
            {
                break;
            }

            var ver = VersionDef.Take(reader);
            versions.Add(ver);

            reader.ExpectSemicolon();
        }

        reader.ExpectEqual();

        var value = Value.Take(reader);

        reader.ExpectSemicolon();

        return new ProgramDef(identifier, versions, value);
    }
}
