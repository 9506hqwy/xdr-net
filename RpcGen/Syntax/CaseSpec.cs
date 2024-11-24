namespace RpcGen;

public class CaseSpec(IList<Value> values, Declaration decl)
{
    public Declaration Declaration { get; } = decl;

    public IList<Value> Values { get; } = values;

    public static CaseSpec Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var values = new List<Value>();
        while (reader.TryExpectCase(out _))
        {
            var value = Value.Take(reader);
            values.Add(value);
            _ = reader.ExpectColon();
        }

        var decl = Declaration.Take(reader);

        _ = reader.ExpectSemicolon();

        return new CaseSpec(values, decl);
    }
}
