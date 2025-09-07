using System.Collections;
using System.Reflection;
using System.Text;

namespace Xdr.Serialization;

public static class XdrDeserializer
{
    public static object Deserialize<T>(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        return DeserializeInternal<T>(value, 0, out rest);
    }

    public static object Deserialize<T>(IEnumerable<byte> value, int count, out IEnumerable<byte> rest)
    {
        return DeserializeInternal<T>(value, count, out rest);
    }

    private static T[] DeserializeArray<T>(IEnumerable<byte> value, int count, out IEnumerable<byte> rest)
    {
        var objs = DeserializeEnumerable<T>(value, count, out rest);
        return [.. objs];
    }

    private static List<T> DeserializeEnumerable<T>(IEnumerable<byte> value, int count, out IEnumerable<byte> rest)
    {
        rest = value;

        var results = new List<T>();
        for (int i = 0; i < count; i++)
        {
            var obj = (T)Deserialize<T>(rest, out rest);
            results.Add(obj);
        }

        return results;
    }

    private static object DeserializeGeneric(string method, Type type, object?[] parameters)
    {
        return typeof(XdrDeserializer)
            .GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)
            .MakeGenericMethod(type)
            .Invoke(null, parameters);
    }

    private static object DeserializeInternal<T>(IEnumerable<byte> value, int count, out IEnumerable<byte> rest)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var type = typeof(T);

        if (type.IsEnum)
        {
            return ReadEnum<T>(value, out rest)!;
        }
        else if (type == typeof(bool))
        {
            return ReadBool(value, out rest);
        }
        else if (type == typeof(short))
        {
            return ReadShort(value, out rest);
        }
        else if (type == typeof(int))
        {
            return ReadInt(value, out rest);
        }
        else if (type == typeof(long))
        {
            return ReadLong(value, out rest);
        }
        else if (type == typeof(ushort))
        {
            return ReadUshort(value, out rest);
        }
        else if (type == typeof(uint))
        {
            return ReadUint(value, out rest);
        }
        else if (type == typeof(ulong))
        {
            return ReadUlong(value, out rest);
        }
        else if (type == typeof(float))
        {
            return ReadFloat(value, out rest);
        }
        else if (type == typeof(double))
        {
            return ReadDouble(value, out rest);
        }
        else if (type == typeof(char))
        {
            return ReadChar(value, out rest);
        }
        else if (type == typeof(string))
        {
            return ReadString(value, out rest);
        }
        else if (type == typeof(byte))
        {
            return ReadByte(value, out rest);
        }
        else if (type == typeof(byte[]))
        {
            return ReadBytes(value, count, out rest);
        }
        else if (typeof(IList<byte>).IsAssignableFrom(type))
        {
            var objs = ReadVariableBytes(value, out rest);
            return Activator.CreateInstance(type, objs);
        }
        else if (typeof(IXdrOption).IsAssignableFrom(type))
        {
            var parameters = new object?[] { value, null };
            var obj = DeserializeGeneric(
                nameof(ReadOption),
                type.GenericTypeArguments.First(),
                parameters);
            rest = (IEnumerable<byte>)parameters[1]!;
            return obj;
        }
        else if (typeof(XdrVoid).IsAssignableFrom(type))
        {
            return ReadVoid(value, out rest);
        }
        else if (typeof(IXdrUnion).IsAssignableFrom(type))
        {
            // TODO: Find XdrUnion<> type.
            var valueType = type.BaseType.GenericTypeArguments.First();
            return ReadUnion<T>(valueType, value, out rest)!;
        }
        else if (type.IsArray)
        {
            var parameters = new object?[] { value, count, null };
            var objs = DeserializeGeneric(
                nameof(DeserializeArray),
                type.GetElementType(),
                parameters);
            rest = (IEnumerable<byte>)parameters[2]!;
            return objs;
        }
        else if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            var len = ReadInt(value, out var temp);
            var parameters = new object?[] { temp, len, null };
            var objs = DeserializeGeneric(
                nameof(DeserializeEnumerable),
                type.GenericTypeArguments.First(),
                parameters);
            rest = (IEnumerable<byte>)parameters[2]!;
            return Activator.CreateInstance(type, objs);
        }
        else if (type.GetCustomAttributes<XdrStructAttribute>().Any())
        {
            return ReadStruct<T>(value, out rest)!;
        }

        throw new NotSupportedException($"Not support type `{type.FullName}`.");
    }

    private static byte[] Read(IEnumerable<byte> value, int count, out IEnumerable<byte> rest)
    {
        rest = value.Skip(count);
        return [.. value.Take(count)];
    }

    private static bool ReadBool(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var v = ReadInt(value, out rest);
        return v != 0;
    }

    private static short ReadShort(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 4, out rest);
        Array.Reverse(bytes);
        return BitConverter.ToInt16(bytes, 0);
    }

    private static int ReadInt(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 4, out rest);
        Array.Reverse(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }

    private static long ReadLong(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 8, out rest);
        Array.Reverse(bytes);
        return BitConverter.ToInt64(bytes, 0);
    }

    private static ushort ReadUshort(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 4, out rest);
        Array.Reverse(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }

    private static uint ReadUint(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 4, out rest);
        Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    private static ulong ReadUlong(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 8, out rest);
        Array.Reverse(bytes);
        return BitConverter.ToUInt64(bytes, 0);
    }

    private static float ReadFloat(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 4, out rest);
        Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }

    private static double ReadDouble(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 8, out rest);
        Array.Reverse(bytes);
        return BitConverter.ToDouble(bytes, 0);
    }

    private static char ReadChar(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 4, out rest);
        Array.Reverse(bytes);
        return BitConverter.ToChar(bytes, 0);
    }

    private static string ReadString(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = ReadVariableBytes(value, out rest);
        return Encoding.UTF8.GetString(bytes);
    }

    private static byte ReadByte(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var bytes = Read(value, 4, out rest);
        Array.Reverse(bytes);
        return bytes[0];
    }

    private static byte[] ReadBytes(IEnumerable<byte> value, int count, out IEnumerable<byte> rest)
    {
        var padding = 4 - (count % 4);

        var bytes = Read(value, count, out rest);

        if (padding < 4)
        {
            rest = rest.Skip(padding);
        }

        return bytes;
    }

    private static byte[] ReadVariableBytes(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var len = ReadInt(value, out rest);
        return ReadBytes(rest, len, out rest);
    }

    private static IXdrOption? ReadOption<T>(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        if (ReadBool(value, out rest))
        {
            var data = (T)Deserialize<T>(rest, out rest);
            return new XdrOption<T>(data);
        }
        else
        {
            return null;
        }
    }

    private static XdrVoid ReadVoid(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        rest = value;
        return XdrVoid.Data;
    }

    private static T ReadEnum<T>(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var v = ReadInt(value, out rest);
        return (T)Enum.ToObject(typeof(T), v);
    }

    private static T ReadStruct<T>(IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var obj = Activator.CreateInstance<T>()!;

        rest = value;

        foreach (var property in Utility.GetXdrStructElement(obj))
        {
            var len = Utility.GetXdrFixedLength(property);
            var parameters = new object?[] { rest, len, null };
            var v = DeserializeGeneric(
                nameof(DeserializeInternal),
                property.PropertyType,
                parameters);
            rest = (IEnumerable<byte>)parameters[2]!;

            property.SetValue(obj, v);
        }

        return obj;
    }

    private static T ReadUnion<T>(Type valueType, IEnumerable<byte> value, out IEnumerable<byte> rest)
    {
        var parameters = new object?[] { value, 0, null };
        var condition = DeserializeGeneric(
            nameof(DeserializeInternal),
            valueType,
            parameters);
        rest = (IEnumerable<byte>)parameters[2]!;

        var obj = (T)Activator.CreateInstance(typeof(T), condition)!;

        var property = Utility.GetXdrUnionElement(obj)
            .FirstOrDefault(p => Utility.MatchXdrUnionArm(p, condition));
        property ??= Utility.GetXdrUnionDefault(obj);

        var len = Utility.GetXdrFixedLength(property);
        parameters = [rest, len, null];
        var propType = property.PropertyType.GenericTypeArguments.First();
        var data = DeserializeGeneric(
            nameof(DeserializeInternal),
            propType,
            parameters);
        rest = (IEnumerable<byte>)parameters[2]!;

        var optionType = typeof(XdrOption<>).MakeGenericType(propType);
        var v = Activator.CreateInstance(optionType, data);

        property.SetValue(obj, v);

        return obj;
    }
}
