namespace RpcGen;

public enum SeparatorType
{
    [KeywordValue("}")]
    BraceEnd,

    [KeywordValue("{")]
    BraceStart,

    [KeywordValue("]")]
    BracketEnd,

    [KeywordValue("[")]
    BracketStart,

    [KeywordValue(")")]
    ParenEnd,

    [KeywordValue("(")]
    ParenStart,

    [KeywordValue(">")]
    AngleBracketEnd,

    [KeywordValue("<")]
    AngleBracketStart,

    [KeywordValue(",")]
    Comma,

    [KeywordValue("=")]
    Equal,

    [KeywordValue(";")]
    Semicolon,

    [KeywordValue(":")]
    Colon,
}
