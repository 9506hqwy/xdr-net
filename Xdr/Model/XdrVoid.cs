﻿namespace Xdr;

[Serializable]
public sealed class XdrVoid
{
    private XdrVoid()
    {
    }

    public static XdrVoid Data { get; } = new XdrVoid();
}
