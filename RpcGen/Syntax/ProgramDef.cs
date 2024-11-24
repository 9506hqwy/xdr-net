namespace RpcGen;

public class ProgramDef(IdentifierToken identifier, IList<VersionDef> versions, Value value) : IDefinition
{
    public IdentifierToken Identifier { get; } = identifier;

    public Value Value { get; } = value;

    public IList<VersionDef> Versions { get; } = versions;

    public static ProgramDef Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var identifier = reader.ExpectIdentifier();

        _ = reader.ExpectBraceStart();

        var versions = new List<VersionDef>();
        while (!reader.Empty)
        {
            if (reader.TryExpectBraceEnd(out _))
            {
                break;
            }

            var ver = VersionDef.Take(reader);
            versions.Add(ver);

            _ = reader.ExpectSemicolon();
        }

        _ = reader.ExpectEqual();

        var value = Value.Take(reader);

        _ = reader.ExpectSemicolon();

        return new ProgramDef(identifier, versions, value);
    }
}
