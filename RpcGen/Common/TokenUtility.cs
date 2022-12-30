namespace RpcGen;

using System.CodeDom;

internal static class TokenUtility
{
    internal static bool GetObject(NumberToken number, out Type type, out object value)
    {
        try
        {
            value = number.GetValue<uint>();
            type = typeof(uint);
            return true;
        }
        catch
        {
            try
            {
                value = number.GetValue<ulong>();
                type = typeof(ulong);
                return true;
            }
            catch
            {
                throw new Exception($"Not supproted number `{number.Value}` ({number.Position}).");
            }
        }
    }

    internal static object GetObject(CodeTypeReference type, NumberToken number)
    {
        if (type.BaseType == typeof(int).FullName)
        {
            return number.GetValue<int>();
        }
        else if (type.BaseType == typeof(uint).FullName)
        {
            return number.GetValue<uint>();
        }
        else if (type.BaseType == typeof(long).FullName)
        {
            return number.GetValue<long>();
        }
        else if (type.BaseType == typeof(ulong).FullName)
        {
            return number.GetValue<ulong>();
        }
        else
        {
            throw new Exception($"Not supproted number `{number.Value}` ({number.Position}).");
        }
    }

    internal static bool IsPrimitive(IList<ReservedToken> reserveds)
    {
        try
        {
            TokenUtility.ToPrimitive(reserveds);
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static Type ToPrimitive(IList<ReservedToken> reserveds)
    {
        var type = string.Join(
            " ",
            reserveds.Select(t => t.Value.ToLowerInvariant()));
        return type switch
        {
            "unsigned" => typeof(uint),
            "unsigned int" => typeof(uint),
            "int" => typeof(int),
            "unsigned hyper" => typeof(ulong),
            "hyper" => typeof(long),
            "float" => typeof(float),
            "double" => typeof(double),
            "bool" => typeof(bool),
            "opaque" => typeof(byte),
            "string" => typeof(string),

            // libvirt extension
            "unsigned char" => typeof(byte),
            "char" => typeof(byte),
            "unsigned short" => typeof(ushort),
            "short" => typeof(short),

            _ => throw new Exception($"Not supported primitive type `{type}`."),
        };
    }

    internal static void ToProperty(
        CodeTypeReference type,
        string ideintifier,
        out CodeMemberField field,
        out CodeMemberProperty prop)
    {
        var fieldName = Utility.ToFieldName(ideintifier);
        field = new CodeMemberField(type, fieldName);

        prop = new CodeMemberProperty();
        prop.Type = type;
        prop.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
        prop.Attributes |= MemberAttributes.Public | MemberAttributes.Final;
        prop.Name = Utility.ToPropertyName(ideintifier);
        prop.GetStatements.Add(
            new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), fieldName)));
        prop.SetStatements.Add(
            new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName),
                new CodePropertySetValueReferenceExpression()));
    }
}
