namespace RpcGen;

using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using Xdr;

public class Generator
{
    private readonly Specification spec;

    public Generator(Specification spec)
    {
        this.spec = spec;
    }

    public void Generate(Stream stream, string nsName)
    {
        var ns = new CodeNamespace(nsName);

        var cls = new CodeTypeDeclaration("Constants");
        cls.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
        cls.Attributes |= MemberAttributes.Public | MemberAttributes.Static;

        ns.Types.Add(cls);

        foreach (var definition in this.spec.Definitions)
        {
            switch (definition)
            {
                case ConstantDef constant:
                    this.AddConstantToCls(cls, constant);
                    break;
                case Declaration decl:
                    break;
                case EnumDef enumd:
                    this.AddClassToNs(ns, enumd);
                    break;
                case StructDef structd:
                    this.AddClassToNs(ns, structd);
                    break;
                case UnionDef uniond:
                    this.AddClassToNs(ns, uniond);
                    break;
                case ProgramDef _:
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        var compileUnit = new CodeCompileUnit();
        compileUnit.Namespaces.Add(ns);

        var provider = new CSharpCodeProvider();

        using var writer = new StreamWriter(stream, leaveOpen: true);
        provider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
        writer.Flush();
    }

    private void AddConstantToCls(CodeTypeDeclaration cls, ConstantDef constant)
    {
        var number = this.GetNumberBy(constant.Constant);
        TokenUtility.GetObject(number, out var type, out var value);

        var field = new CodeMemberField(type, Utility.ToPropertyName(constant.Identifier.Value));
        field.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
        field.Attributes |= MemberAttributes.Public | MemberAttributes.Const;
        field.InitExpression = new CodePrimitiveExpression(value);

        cls.Members.Add(field);
    }

    private void AddPropertyToCls(CodeTypeDeclaration cls, CaseSpec cased, Declaration condition, ref int voidCount)
    {
        var decl = cased.Declaration;

        var type = new CodeTypeReference(typeof(XdrOption<>));
        type.TypeArguments.Add(this.ToType(decl, out var fixedValue));

        var identifier = decl.Identifier;
        if (decl.Type.IsVoid)
        {
            identifier += voidCount.ToString();
            voidCount += 1;
        }

        TokenUtility.ToProperty(type, identifier, out var field, out var prop);

        foreach (var value in cased.Values)
        {
            CodeExpression? expr = null;
            var conditionType = this.ToType(condition, out var _);
            if (condition.Type.IsBool)
            {
                if (value.IsTrue)
                {
                    expr = new CodePrimitiveExpression(true);
                }
                else if (value.IsFalse)
                {
                    expr = new CodePrimitiveExpression(false);
                }
                else
                {
                    var ident = value.Identifier?.Value ?? this.GetNumberBy(value.Constant!).Value;
                    throw new Exception($"Invalid bool value `{ident}`.");
                }
            }
            else if (this.IsPrimitive(condition.Type))
            {
                var number = this.GetNumberBy(value);
                var val = TokenUtility.GetObject(conditionType, number);
                expr = new CodePrimitiveExpression(val);
            }
            else if (condition.Type.Identifier is not null)
            {
                expr = new CodeFieldReferenceExpression(
                    new CodeTypeReferenceExpression(conditionType),
                    Utility.ToPropertyName(value.Identifier!.Value));
            }
            else
            {
                throw new InvalidOperationException();
            }

            prop.CustomAttributes.Add(new CodeAttributeDeclaration(
                typeof(XdrUnionCaseAttribute).FullName!,
                new CodeAttributeArgument(expr!)));
        }

        if (fixedValue is not null)
        {
            prop.CustomAttributes.Add(new CodeAttributeDeclaration(
                typeof(XdrFixedLengthAttribute).FullName!,
                new CodeAttributeArgument(new CodePrimitiveExpression(fixedValue))));
        }

        cls.Members.Add(field);
        cls.Members.Add(prop);
    }

    private void AddPropertyToCls(CodeTypeDeclaration cls, Declaration decl, ref int voidCount)
    {
        var type = new CodeTypeReference(typeof(XdrOption<>));
        type.TypeArguments.Add(this.ToType(decl, out var fixedValue));

        var identifier = decl.Identifier;
        if (decl.Type.IsVoid)
        {
            identifier += voidCount.ToString();
            voidCount += 1;
        }

        TokenUtility.ToProperty(type, identifier, out var field, out var prop);

        prop.CustomAttributes.Add(new CodeAttributeDeclaration(
            typeof(XdrUnionDefaultAttribute).FullName!));
        if (fixedValue is not null)
        {
            prop.CustomAttributes.Add(new CodeAttributeDeclaration(
                typeof(XdrFixedLengthAttribute).FullName!,
                new CodeAttributeArgument(new CodePrimitiveExpression(fixedValue))));
        }

        cls.Members.Add(field);
        cls.Members.Add(prop);
    }

    private void AddPropertyToCls(CodeTypeDeclaration cls, Declaration decl, int idx)
    {
        TokenUtility.ToProperty(
            this.ToType(decl, out var fixedValue),
            decl.Identifier,
            out var field,
            out var prop);

        prop.CustomAttributes.Add(new CodeAttributeDeclaration(
            typeof(XdrElementOrderAttribute).FullName!,
            new CodeAttributeArgument(new CodePrimitiveExpression(idx))));
        if (fixedValue is not null)
        {
            prop.CustomAttributes.Add(new CodeAttributeDeclaration(
                typeof(XdrFixedLengthAttribute).FullName!,
                new CodeAttributeArgument(new CodePrimitiveExpression(fixedValue))));
        }

        cls.Members.Add(field);
        cls.Members.Add(prop);
    }

    private void AddClassToNs(CodeNamespace ns, EnumDef enumd)
    {
        var cls = new CodeTypeDeclaration(Utility.ToClassName(enumd.Identifier.Value))
        {
            IsEnum = true,
        };
        cls.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(System.SerializableAttribute).FullName!));

        foreach ((var field, var value) in enumd.Body)
        {
            var number = this.GetNumberBy(value);
            var val = number.GetValue<int>();

            var f = new CodeMemberField(typeof(int), Utility.ToPropertyName(field.Value));
            f.InitExpression = new CodePrimitiveExpression(val);

            cls.Members.Add(f);
        }

        ns.Types.Add(cls);
    }

    private void AddClassToNs(CodeNamespace ns, StructDef structd)
    {
        var cls = new CodeTypeDeclaration(Utility.ToClassName(structd.Identifier.Value))
        {
            IsPartial = true,
        };
        cls.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(System.SerializableAttribute).FullName!));
        cls.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(XdrStructAttribute).FullName!));

        for (int i = 0; i < structd.Body.Count; i++)
        {
            this.AddPropertyToCls(cls, structd.Body[i], i + 1);
        }

        ns.Types.Add(cls);
    }

    private void AddClassToNs(CodeNamespace ns, UnionDef uniond)
    {
        var conitionType = this.ToType(uniond.Condition, out var _);

        var xdrUnionType = new CodeTypeReference(typeof(XdrUnion<>));
        xdrUnionType.TypeArguments.Add(conitionType);

        var cls = new CodeTypeDeclaration(Utility.ToClassName(uniond.Identifier.Value))
        {
            IsPartial = true,
        };
        cls.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(System.SerializableAttribute).FullName!));
        cls.BaseTypes.Add(xdrUnionType);

        var ctor = new CodeConstructor();
        ctor.Attributes = MemberAttributes.Public;
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(conitionType, "value"));
        ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("value"));
        cls.Members.Add(ctor);

        var voidCount = 0;
        foreach (var cased in uniond.Cases)
        {
            this.AddPropertyToCls(cls, cased, uniond.Condition, ref voidCount);
        }

        if (uniond.DefaultValue is not null)
        {
            this.AddPropertyToCls(cls, uniond.DefaultValue, ref voidCount);
        }

        ns.Types.Add(cls);
    }

    private NumberToken GetNumberBy(Value value)
    {
        if (value.Identifier is not null)
        {
            return this.GetNumberBy(value.Identifier);
        }
        else if (value.Constant is not null)
        {
            return this.GetNumberBy(value.Constant);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private NumberToken GetNumberBy(Constant constant)
    {
        if (constant.Number is not null)
        {
            return constant.Number;
        }
        else if (constant.Identifier is not null)
        {
            return this.GetNumberBy(constant.Identifier);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private NumberToken GetNumberBy(IdentifierToken token)
    {
        var def = this.spec.Definitions
            .OfType<ConstantDef>()
            .FirstOrDefault(c => c.Identifier.Value == token.Value);
        if (def is null)
        {
            throw new Exception($"Not found identifier `{token.Value}` ({token.Position}).");
        }

        return this.GetNumberBy(def.Constant);
    }

    private bool IsPrimitive(TypeSpecifier type)
    {
        if (type.IsVoid)
        {
            return false;
        }
        else if (type.Identifier is not null)
        {
            var alias = this.spec.Definitions
                .OfType<Declaration>()
                .FirstOrDefault(d => d.Identifier == type.Identifier.Value);
            if (alias is not null)
            {
                return this.IsPrimitive(alias.Type);
            }

            return false;
        }

        return TokenUtility.IsPrimitive(type.Reserved!);
    }

    private CodeTypeReference ToType(Declaration decl, out int? fixedValue)
    {
        var type = this.ToType(decl.Type, out fixedValue);

        if (decl.Optional)
        {
            var option = new CodeTypeReference(typeof(XdrOption<>));
            option.TypeArguments.Add(type);
            return option;
        }
        else if (type.BaseType == typeof(string).FullName)
        {
            return type;
        }
        else if (decl.Variable)
        {
            var list = new CodeTypeReference(typeof(List<>));
            list.TypeArguments.Add(type);
            return list;
        }
        else if (decl.Fixed is not null)
        {
            var arr = new CodeTypeReference();
            arr.ArrayElementType = type;
            arr.ArrayRank = 1;

            var number = this.GetNumberBy(decl.Fixed);
            fixedValue = (int)number.GetValue<int>();

            return arr;
        }

        return type;
    }

    private CodeTypeReference ToType(TypeSpecifier type, out int? fixedValue)
    {
        fixedValue = null;

        if (type.IsVoid)
        {
            return new CodeTypeReference(typeof(XdrVoid));
        }
        else if (type.Identifier is not null)
        {
            var alias = this.spec.Definitions
                .OfType<Declaration>()
                .FirstOrDefault(d => d.Identifier == type.Identifier.Value);
            if (alias is not null)
            {
                return this.ToType(alias, out fixedValue);
            }

            return new CodeTypeReference(Utility.ToClassName(type.Identifier.Value));
        }

        return new CodeTypeReference(TokenUtility.ToPrimitive(type.Reserved!));
    }
}
