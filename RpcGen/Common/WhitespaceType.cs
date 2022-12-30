namespace RpcGen;

public enum WhitespaceType
{
    [KeywordValue(" ")]
    Ws,

    [KeywordValue("\r")]
    Cr,

    [KeywordValue("\n")]
    Lf,

    [KeywordValue("\t")]
    Tab,
}
