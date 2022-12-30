namespace RpcGen;

public enum ReservedType
{
    [KeywordValue("case")]
    KeywordCase,

    [KeywordValue("const")]
    KeywordConst,

    [KeywordValue("default")]
    KeywordDefault,

    [KeywordValue("enum")]
    KeywordEnum,

    [KeywordValue("struct")]
    KeywordStruct,

    [KeywordValue("switch")]
    KeywordSwitch,

    [KeywordValue("typedef")]
    KeywordTypedef,

    [KeywordValue("union")]
    KeywordUnion,

    [KeywordValue("bool")]
    TypeBool,

    [KeywordValue("double")]
    TypeDouble,

    [KeywordValue("float")]
    TypeFloat,

    [KeywordValue("hyper")]
    TypeHyper,

    [KeywordValue("int")]
    TypeInt,

    [KeywordValue("opaque")]
    TypeOpaque,

    [KeywordValue("quadruple")]
    TypeQuadruple,

    [KeywordValue("string")]
    TypeString,

    [KeywordValue("unsigned")]
    TypeUnsigned,

    [KeywordValue("void")]
    TypeVoid,

    /* libvirt extenstion */

    [KeywordValue("char")]
    TypeChar,

    [KeywordValue("short")]
    TypeShort,

    /* rpcl extenstion */

    [KeywordValue("program")]
    KeywordProgram,

    [KeywordValue("version")]
    KeywordVersion,
}
