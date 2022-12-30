namespace RpcGen;

public class Specification
{
    public Specification(IDefinition[] definitions)
    {
        this.Definitions = definitions;
    }

    public IDefinition[] Definitions { get; }

    public static Specification Take(TokenReader reader)
    {
        var defs = new List<IDefinition>();

        reader.Next();
        while (!reader.Empty)
        {
            var token = reader.ExpectReserved();
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
                    reader.ExpectSemicolon();
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
        }

        return new Specification(defs.ToArray());
    }
}
