namespace RpcGen;

public class VersionDef(IdentifierToken identifier, IList<ProcedureDef> procedures, Value value)
{
    public IdentifierToken Identifier { get; } = identifier;

    public IList<ProcedureDef> Procedures { get; } = procedures;

    public Value Value { get; } = value;

    public static VersionDef Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        _ = reader.ExpectVersion();

        var identifier = reader.ExpectIdentifier();

        _ = reader.ExpectBraceStart();

        var procedures = new List<ProcedureDef>();
        while (!reader.Empty)
        {
            if (reader.TryExpectBraceEnd(out _))
            {
                break;
            }

            var proc = ProcedureDef.Take(reader);
            procedures.Add(proc);

            _ = reader.ExpectSemicolon();
        }

        _ = reader.ExpectEqual();

        var value = Value.Take(reader);

        return new VersionDef(identifier, procedures, value);
    }
}
