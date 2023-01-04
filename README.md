# XDR (RFC 4506) for .Net

This library is XDR (RFC 4506) serializer and deserializer for .NET Standard 2.0,
and stub code generation tool from interface defined file.

## Usage

```csharp
var bytes = XdrSerializer.Serialize(1);
var val = XdrDeserializer.Deserialize<int>(bytes);
```

## Data Types

| XDR                         | C#                                             |
| --------------------------- | ---------------------------------------------- |
| Integer                     | short, int                                     |
| Unsigned Integer            | ushort, uint, bool                             |
| Enumeration                 | enum                                           |
| Hyper Integer               | long                                           |
| Unsigned Hyper Integer      | ulong                                          |
| Floatinng-Point             | float                                          |
| Double-Precision            | double                                         |
| Quadruple-Precision         |                                                |
| Fixed-Length Opaque Data    | byte[] [^1]                                    |
| Variable-Length Opaque Data | IList\<byte>                                   |
| String                      | string                                         |
| Fixed-Length Arary          | T[] [^1]                                       |
| Variable-Length Array       | IList\<T>                                      |
| Structure                   | class                                          |
| Discriminated Union         | XdrUnion\<T>                                   |
| Void                        | XdrVoid                                        |
| Optional-Data               | XdrOption\<T>                                  |

[^1]: need `[Xdr.XdrFixedLength(N)]` attribute to struct property.

### Structure

XDR structure is mapped to bellow C# code.

```
struct file {
   string filename<256>;
   filetype type;
   opaque owner[10];
   opaque data<1024>;
};
```

```csharp
[Xdr.XdrStruct]
class File
{
    [Xdr.XdrElementOrder(1)]
    public string Filename { get; set; }

    [Xdr.XdrElementOrder(2)]
    public Filetype @Type { get; set; }

    [Xdr.XdrElementOrder(3)]
    [Xdr.XdrFixedLength(10)]
    public byte[] Owner { get; set; }

    [Xdr.XdrElementOrder(4)]
    public List<byte> Data { get; set; }
}
```

### Discriminated Union

XDR union is mapped to bellow C# code.

```
enum filekind {
   TEXT = 0,
   DATA = 1,
   EXEC = 2
};

union filetype switch (filekind kind) {
case TEXT:
   void;
case DATA:
   string creator<10>;
case EXEC:
   string interpretor<10>;
};
```

```csharp
class FileType : XdrUnion<int>
{
    public FileType(int value)
        : base(value)
    {
    }

    [Xdr.XdrUnionCase(0)]
    public XdrOption<XdrVoid>? Void { get; set; }

    [Xdr.XdrUnionCase(1)]
    public XdrOption<string>? Creator { get; set; }

    [Xdr.XdrUnionCase(2)]
    public XdrOption<string>? Interpretor { get; set; }
}
```

## Stub Code Generation

```sh
dotnet rpc-gen <PATH>
```

PATH is interface defined file path.

## References

- [XDR: External Data Representation Standard](https://www.rfc-editor.org/rfc/rfc4506.txt)
