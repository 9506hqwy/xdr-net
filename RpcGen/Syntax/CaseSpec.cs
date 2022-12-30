namespace RpcGen;

public class CaseSpec
{
    public CaseSpec(IList<Value> values, Declaration decl)
    {
        this.Values = values;
        this.Declaration = decl;
    }

    public Declaration Declaration { get; }

    public IList<Value> Values { get; }

    public static CaseSpec Take(TokenReader reader)
    {
        var values = new List<Value>();
        while (reader.TryExpectCase(out var _))
        {
            var value = Value.Take(reader);
            values.Add(value);
            reader.ExpectColon();
        }

        var decl = Declaration.Take(reader);

        reader.ExpectSemicolon();

        return new CaseSpec(values, decl);
    }
}
