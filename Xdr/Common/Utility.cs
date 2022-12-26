namespace Xdr;

using System.Reflection;

internal static class Utility
{
    internal static int GetXdrFixedLength(PropertyInfo prop)
    {
        var attr = prop.GetCustomAttribute<XdrFixedLengthAttribute>();
        return attr?.Length ?? default;
    }

    internal static PropertyInfo GetXdrUnionDefault(object value)
    {
        return value.GetType()
            .GetProperties()
            .FirstOrDefault(Utility.IsXdrDefault);
    }

    internal static PropertyInfo[] GetXdrStructElement(object value)
    {
        var elements = new SortedDictionary<int, PropertyInfo>();

        foreach (var property in value.GetType().GetProperties())
        {
            var attr = property.GetCustomAttribute<XdrElementOrderAttribute>();
            if (attr is null)
            {
                continue;
            }

            if (elements.ContainsKey(attr.Order))
            {
                throw new InvalidOperationException();
            }

            elements.Add(attr.Order, property);
        }

        return elements.Values.ToArray();
    }

    internal static PropertyInfo[] GetXdrUnionElement(object value)
    {
        var elements = new Dictionary<int, PropertyInfo>();

        foreach (var property in value.GetType().GetProperties())
        {
            var attr = property.GetCustomAttribute<XdrUnionCaseAttribute>();
            if (attr is null)
            {
                continue;
            }

            if (elements.ContainsKey(attr.Value))
            {
                throw new InvalidOperationException();
            }

            elements.Add(attr.Value, property);
        }

        return elements.Values.ToArray();
    }

    internal static bool IsXdrDefault(PropertyInfo prop)
    {
        return prop.IsDefined(typeof(XdrUnionDefaultAttribute), true);
    }

    internal static bool MatchXdrUnionArm(PropertyInfo prop, int value)
    {
        var attr = prop.GetCustomAttribute<XdrUnionArmAttribute>();
        return attr != null && attr.Value == value;
    }
}
