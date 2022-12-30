﻿namespace RpcGen;

using System.Text;

public class ProtoReader : TextReader
{
    private const int EOS = -1;

    private const char NEWLINE = '\n';

    private readonly Stream stream;

    private int currentByte = '\0';

    public ProtoReader(Stream stream)
    {
        this.stream = stream;
        this.NextLinePosition();
    }

    public int CurrentLine { get; private set; }

    public int CurrentColumn { get; private set; }

    public bool EndOfStream => this.currentByte == ProtoReader.EOS;

    public Position GetReadPosition()
    {
        return new Position
        {
            Column = this.CurrentColumn,
            Line = this.CurrentLine,
        };
    }

    public override int Peek()
    {
        var b = this.stream.ReadByte();

        if (b > ProtoReader.EOS)
        {
            this.stream.Seek(-1, SeekOrigin.Current);
        }

        return b;
    }

    public override int Read()
    {
        this.currentByte = this.stream.ReadByte();

        if (this.currentByte > ProtoReader.EOS)
        {
            if (this.currentByte == ProtoReader.NEWLINE)
            {
                this.NextLinePosition();
            }
            else
            {
                this.CurrentColumn += 1;
            }
        }

        return this.currentByte;
    }

    public void Skip(int count)
    {
        for (int i = 0; i < count; i++)
        {
            this.Read();
        }
    }

    public void SkipWhileAny(int[] matches)
    {
        while (this.StartsWithAny(matches))
        {
            this.Read();
        }
    }

    public bool StartsWith(Predicate<int> matcher)
    {
        var b = this.Peek();
        return matcher(b);
    }

    public bool StartsWith(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);

        var buf = new byte[bytes.Length];
        var num = this.stream.Read(buf, 0, bytes.Length);

        this.stream.Seek(num * -1, SeekOrigin.Current);

        return (num == bytes.Length) ?
            bytes.Zip(buf).All(v => v.First == v.Second) :
            false;
    }

    public bool StartsWithAny(int[] values)
    {
        var b = this.Peek();
        return values.Contains(b);
    }

    private void NextLinePosition()
    {
        this.CurrentLine += 1;
        this.CurrentColumn = 1;
    }
}
