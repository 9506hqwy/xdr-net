namespace RpcGen;

public class Specification(IDefinition[] definitions)
{
#pragma warning disable CA1819
    public IDefinition[] Definitions { get; } = definitions;
#pragma warning restore CA1819

    public static Specification Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var defs = new List<IDefinition>();

        _ = reader.Next();
        while (!reader.Empty)
        {
            var token = reader.ExpectReserved();
#pragma warning disable IDE0010
            switch (token.Type)
            {
                case ReservedType.KeywordEnum:
                    var enumd = EnumDef.Take(reader);
                    defs.Add(enumd);
                    break;
                case ReservedType.KeywordStruct:
                    var structd = StructDef.Take(reader);
                    defs.Add(structd);
                    break;
                case ReservedType.KeywordTypedef:
                    var decl = Declaration.Take(reader);
                    _ = reader.ExpectSemicolon();
                    defs.Add(decl);
                    break;
                case ReservedType.KeywordUnion:
                    var uniond = UnionDef.Take(reader);
                    defs.Add(uniond);
                    break;
                case ReservedType.KeywordConst:
                    var constantd = ConstantDef.Take(reader);
                    defs.Add(constantd);
                    break;
                case ReservedType.KeywordProgram:
                    var program = ProgramDef.Take(reader);
                    defs.Add(program);
                    break;
                default:
                    throw new Exception($"Not supported keyword {token.Value} ({token.Position}).");
            }
#pragma warning restore IDE0010
        }

        return new Specification([.. defs]);
    }
}
