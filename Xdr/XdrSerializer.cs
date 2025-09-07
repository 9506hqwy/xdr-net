using System.Collections;
using System.Reflection;
using System.Text;

namespace Xdr.Serialization;

public static class XdrSerializer
{
    public static byte[] Serialize(bool value)
    {
        int v = value ? 1 : 0;
        return Serialize(v);
    }

    public static byte[] Serialize(short value)
    {
        return Serialize((int)value);
    }

    public static byte[] Serialize(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        return bytes;
    }

    public static byte[] Serialize(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        return bytes;
    }

    public static byte[] Serialize(ushort value)
    {
        return Serialize((uint)value);
    }

    public static byte[] Serialize(uint value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        return bytes;
    }

    public static byte[] Serialize(ulong value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        return bytes;
    }

    public static byte[] Serialize(float value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        return bytes;
    }

    public static byte[] Serialize(double value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        return bytes;
    }

    public static byte[] Serialize(string value)
    {
        var v = Encoding.UTF8.GetBytes(value);
        return Serialize(v.ToList());
    }

    public static byte[] Serialize(byte[] value)
    {
        return SerializeEnumerate(value, false);
    }

    public static byte[] Serialize(IList<byte> value)
    {
        return SerializeEnumerate(value, true);
    }

    public static byte[] Serialize<T>(T[] value)
    {
        return SerializeFixedArray(value);
    }

    public static byte[] Serialize<T>(IList<T> value)
    {
        return SerializeVariableArray(value);
    }

    public static byte[] Serialize(IXdrOption? value)
    {
        return value is null
            ? Serialize(0)
            : [.. Serialize(1)
,
                .. Serialize(value.Data)];
    }

#pragma warning disable IDE0060
    public static byte[] Serialize(XdrVoid value)
#pragma warning restore IDE0060
    {
        return [];
    }

    public static byte[] Serialize(IXdrUnion value)
    {
        var bytes = Serialize(value.Data);

        var property = Utility.GetXdrUnionElement(value)
            .FirstOrDefault(p => Utility.MatchXdrUnionArm(p, value.Data));
        property ??= Utility.GetXdrUnionDefault(value);

        var propValue = property.GetValue(value) as IXdrOption;
        return [.. bytes, .. Serialize(propValue!.Data)];
    }

    public static byte[] Serialize(object? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var type = value.GetType();
        if (type.IsEnum)
        {
            int v = (int)value;
            return Serialize(v);
        }

        switch (value)
        {
            case bool v: return Serialize(v);
            case short v: return Serialize(v);
            case int v: return Serialize(v);
            case long v: return Serialize(v);
            case ushort v: return Serialize(v);
            case uint v: return Serialize(v);
            case ulong v: return Serialize(v);
            case float v: return Serialize(v);
            case double v: return Serialize(v);
            case string v: return Serialize(v);
            case byte[] v: return Serialize(v);
            case IList<byte> v: return Serialize(v);
            case IXdrOption v: return Serialize(v);
            case XdrVoid v: return Serialize(v);
            case IXdrUnion v: return Serialize(v);
            default: break;
        }

        return type.IsArray
            ? (byte[])typeof(XdrSerializer)
                .GetMethod(nameof(SerializeFixedArray), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(type.GetElementType())
                .Invoke(null, [value])
            : typeof(IEnumerable).IsAssignableFrom(type)
            ? (byte[])typeof(XdrSerializer)
                .GetMethod(nameof(SerializeVariableArray), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(type.GenericTypeArguments.First())
                .Invoke(null, [value])
            : type.GetCustomAttributes<XdrStructAttribute>().Any()
            ? SerializeStruct(value)
            : throw new NotSupportedException($"Not support type `{type.FullName}`.");
    }

    private static byte[] Serialize(Type type, object value)
    {
        if (typeof(IXdrOption).IsAssignableFrom(type))
        {
            var option = (IXdrOption)value;
            return Serialize(option);
        }
        else
        {
            return Serialize(value);
        }
    }

    private static byte[] SerializeEnumerate(IEnumerable<byte> value, bool prefixLength)
    {
        int len = value.Count();
        int paddingLen = 4 - (len % 4);

        var bytes = prefixLength ? Serialize(len) : [];
        bytes = [.. bytes, .. value];

        if (paddingLen < 4)
        {
            var padding = new byte[paddingLen];
            bytes = [.. bytes, .. padding];
        }

        return bytes;
    }

    private static byte[] SerializeFixedArray<T>(T[] value)
    {
        return [.. value
            .Cast<object>()
            .SelectMany(o => Serialize(typeof(T), o))];
    }

    private static byte[] SerializeStruct(object value)
    {
        return [.. Utility.GetXdrStructElement(value).SelectMany(p => SerializeStructElement(p, value))];
    }

    private static byte[] SerializeStructElement(PropertyInfo property, object obj)
    {
        var value = property.GetValue(obj);
        return Serialize(property.PropertyType, value);
    }

    private static byte[] SerializeVariableArray<T>(IList<T> value)
    {
        int len = value.Count;

        var bytes = Serialize(len);
        var arr = value
            .Cast<object>()
            .SelectMany(o => Serialize(typeof(T), o))
            .ToArray();

        return [.. bytes, .. arr];
    }
}
