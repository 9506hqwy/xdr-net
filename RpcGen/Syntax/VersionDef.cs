namespace RpcGen;

public class VersionDef
{
    public VersionDef(IdentifierToken identifier, IList<ProcedureDef> procedures, Value value)
    {
        this.Identifier = identifier;
        this.Procedures = procedures;
        this.Value = value;
    }

    public IdentifierToken Identifier { get; }

    public IList<ProcedureDef> Procedures { get; }

    public Value Value { get; }

    public static VersionDef Take(TokenReader reader)
    {
        reader.ExpectVersion();

        var identifier = reader.ExpectIdentifier();

        reader.ExpectBraceStart();

        var procedures = new List<ProcedureDef>();
        while (!reader.Empty)
        {
            if (reader.TryExpectBraceEnd(out var _))
            {
                break;
            }

            var proc = ProcedureDef.Take(reader);
            procedures.Add(proc);

            reader.ExpectSemicolon();
        }

        reader.ExpectEqual();

        var value = Value.Take(reader);

        return new VersionDef(identifier, procedures, value);
    }
}
