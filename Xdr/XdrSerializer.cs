namespace Xdr.Serialization;

using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

public static class XdrSerializer
{
    public static byte[] Serialize(bool value)
    {
        int v = value ? 1 : 0;
        return XdrSerializer.Serialize(v);
    }

    public static byte[] Serialize(short value)
    {
        return XdrSerializer.Serialize((int)value);
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
        return XdrSerializer.Serialize((uint)value);
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
        return XdrSerializer.Serialize(v.ToList());
    }

    public static byte[] Serialize(byte[] value)
    {
        return XdrSerializer.SerializeEnumerate(value, false);
    }

    public static byte[] Serialize(IList<byte> value)
    {
        return XdrSerializer.SerializeEnumerate(value, true);
    }

    public static byte[] Serialize<T>(T[] value)
    {
        return XdrSerializer.SerializeFixedArray(value);
    }

    public static byte[] Serialize<T>(IList<T> value)
    {
        return XdrSerializer.SerializeVariableArray(value);
    }

    public static byte[] Serialize(IXdrOption? value)
    {
        if (value is null)
        {
            return XdrSerializer.Serialize(0);
        }
        else
        {
            return XdrSerializer.Serialize(1)
                .Concat(XdrSerializer.Serialize(value.Data))
                .ToArray();
        }
    }

#pragma warning disable IDE0060
    public static byte[] Serialize(XdrVoid value)
#pragma warning restore IDE0060
    {
        return Array.Empty<byte>();
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
            return XdrSerializer.Serialize(v);
        }

        switch (value)
        {
            case bool v: return XdrSerializer.Serialize(v);
            case short v: return XdrSerializer.Serialize(v);
            case int v: return XdrSerializer.Serialize(v);
            case long v: return XdrSerializer.Serialize(v);
            case ushort v: return XdrSerializer.Serialize(v);
            case uint v: return XdrSerializer.Serialize(v);
            case ulong v: return XdrSerializer.Serialize(v);
            case float v: return XdrSerializer.Serialize(v);
            case double v: return XdrSerializer.Serialize(v);
            case string v: return XdrSerializer.Serialize(v);
            case byte[] v: return XdrSerializer.Serialize(v);
            case IList<byte> v: return XdrSerializer.Serialize(v);
            case IXdrOption v: return XdrSerializer.Serialize(v);
            case XdrVoid v: return XdrSerializer.Serialize(v);
            default: break;
        }

        if (type.IsArray)
        {
            return (byte[])typeof(XdrSerializer)
                .GetMethod(nameof(SerializeFixedArray), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(type.GetElementType())
                .Invoke(null, new object[] { value });
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            return (byte[])typeof(XdrSerializer)
                .GetMethod(nameof(SerializeVariableArray), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(type.GenericTypeArguments.First())
                .Invoke(null, new object[] { value });
        }

        if (type.GetCustomAttributes<XdrStructAttribute>().Any())
        {
            return XdrSerializer.SerializeStruct(value);
        }

        if (type.GetCustomAttributes<XdrUnionAttribute>().Any())
        {
            return XdrSerializer.SerializeUnion(value);
        }

        throw new NotSupportedException($"Not support type `{type.FullName}`.");
    }

    private static byte[] SerializeEnumerate(IEnumerable<byte> value, bool prefixLength)
    {
        int len = value.Count();
        int paddingLen = 4 - (len % 4);

        var bytes = prefixLength ? XdrSerializer.Serialize(len) : Array.Empty<byte>();
        bytes = bytes.Concat(value).ToArray();

        if (paddingLen < 4)
        {
            var padding = new byte[paddingLen];
            bytes = bytes.Concat(padding).ToArray();
        }

        return bytes;
    }

    private static byte[] SerializeFixedArray<T>(T[] value)
    {
        return value
            .Cast<object>()
            .SelectMany(XdrSerializer.Serialize)
            .ToArray();
    }

    private static byte[] SerializeStruct(object value)
    {
        return Utility.GetXdrStructElement(value)
            .Select(p => p.GetValue(value))
            .SelectMany(XdrSerializer.Serialize)
            .ToArray();
    }

    private static byte[] SerializeUnion(object value)
    {
        foreach (var property in Utility.GetXdrUnionElement(value))
        {
            var v = property.GetValue(value);
            if (v is null)
            {
                continue;
            }

            return XdrSerializer.SerializeUnionValue<XdrUnionCaseAttribute>(value, property);
        }

        var defaultProperty = Utility.GetXdrUnionDefault(value);
        return XdrSerializer.SerializeUnionValue<XdrUnionDefaultAttribute>(value, defaultProperty);
    }

    private static byte[] SerializeUnionValue<T>(object value, PropertyInfo property)
        where T : XdrUnionArmAttribute
    {
        var propValue = property.GetValue(value) as IXdrOption;

        var attr = property.GetCustomAttribute<T>();
        var bytes = XdrSerializer.Serialize(attr.Value);

        return bytes.Concat(XdrSerializer.Serialize(propValue!.Data)).ToArray();
    }

    private static byte[] SerializeVariableArray<T>(IList<T> value)
    {
        int len = value.Count;

        var bytes = XdrSerializer.Serialize(len);
        var arr = value
            .Cast<object>()
            .SelectMany(XdrSerializer.Serialize)
            .ToArray();

        return bytes.Concat(arr).ToArray();
    }
}
