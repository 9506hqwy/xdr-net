namespace RpcGen;

using System.Reflection;

public class Keyword
{
    private static readonly Keyword[] Reserved;

    private static readonly Keyword[] Separator;

    private static readonly Keyword[] Whitespace;

    static Keyword()
    {
        Keyword.Reserved = Keyword.Enumerate(typeof(ReservedType)).OrderBy(k => k.Value).ToArray();
        Keyword.Separator = Keyword.Enumerate(typeof(SeparatorType)).OrderBy(k => k.Value).ToArray();
        Keyword.Whitespace = Keyword.Enumerate(typeof(WhitespaceType)).OrderBy(k => k.Value).ToArray();
    }

    private Keyword(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }

    public static int[] SeparatorChars => Keyword.Convert(Keyword.Separator);

    public static int[] WhiteSpaceChars => Keyword.Convert(Keyword.Whitespace);

    public string Name { get; }

    public string Value { get; }

    public static bool MatchReserved(string value, out Keyword? matched)
    {
        foreach (var keyword in Keyword.Reserved)
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
        return Keyword.TryFind(reader, Keyword.Separator, out matched);
    }

    public static bool MatchWhitespace(ProtoReader reader, out Keyword? matched)
    {
        return Keyword.TryFind(reader, Keyword.Whitespace, out matched);
    }

    private static int[] Convert(Keyword[] keywords)
    {
        return keywords.SelectMany(k => k.Value.ToCharArray()).Select(c => (int)c).ToArray();
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
