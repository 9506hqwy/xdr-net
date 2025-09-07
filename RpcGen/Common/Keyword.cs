using System.Reflection;

namespace RpcGen;

public class Keyword
{
    private static readonly Keyword[] Reserved = [.. Enumerate(typeof(ReservedType)).OrderBy(k => k.Value)];

    private static readonly Keyword[] Separator = [.. Enumerate(typeof(SeparatorType)).OrderBy(k => k.Value)];

    private static readonly Keyword[] Whitespace = [.. Enumerate(typeof(WhitespaceType)).OrderBy(k => k.Value)];

    private Keyword(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }

    internal static int[] SeparatorChars => Convert(Separator);

    internal static int[] WhiteSpaceChars => Convert(Whitespace);

    public string Name { get; }

    public string Value { get; }

    public static bool MatchReserved(string value, out Keyword? matched)
    {
        foreach (var keyword in Reserved)
        {
            if (value == keyword.Value)
            {
                matched = keyword;
                return true;
            }
        }

        matched = null;
        return false;
    }

    public static bool MatchSeparator(ProtoReader reader, out Keyword? matched)
    {
        ArgumentNullException.ThrowIfNull(reader);
        return TryFind(reader, Separator, out matched);
    }

    public static bool MatchWhitespace(ProtoReader reader, out Keyword? matched)
    {
        ArgumentNullException.ThrowIfNull(reader);
        return TryFind(reader, Whitespace, out matched);
    }

    private static int[] Convert(Keyword[] keywords)
    {
        return [.. keywords.SelectMany(k => k.Value.ToCharArray()).Select(c => (int)c)];
    }

    private static IEnumerable<Keyword> Enumerate(Type type)
    {
        foreach (var field in type.GetFields())
        {
            foreach (var attr in field.GetCustomAttributes<KeywordValueAttribute>())
            {
                yield return new Keyword(field.Name, attr.Value);
            }
        }
    }

    private static bool TryFind(ProtoReader reader, Keyword[] keywords, out Keyword? found)
    {
        foreach (var keyword in keywords)
        {
            if (reader.StartsWith(keyword.Value))
            {
                found = keyword;
                return true;
            }
        }

        found = null;
        return false;
    }
}
