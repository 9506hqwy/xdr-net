using System.Text;
using System.Text.RegularExpressions;

namespace RpcGen;

public class Lexer(ProtoReader reader)
{
    private const int EOS = -1;

    private const string CommentStart = "/*";
    private const string CommentEnd = "*/";
    private const string CommentLine = "%";

    private readonly ProtoReader reader = reader;

    public IEnumerable<Token> Enumerate()
    {
        while (this.reader.Peek() != -1)
        {
            if (this.reader.StartsWith(CommentStart))
            {
                var position = this.reader.GetReadPosition();
                this.reader.Skip(CommentStart.Length);
                yield return this.GetCommentBlockToken(position);
            }
            else if (this.reader.StartsWith(CommentLine))
            {
                var position = this.reader.GetReadPosition();
                this.reader.Skip(CommentLine.Length);
                yield return this.GetCommentLineToken(position);
            }
            else if (Keyword.MatchWhitespace(this.reader, out var _))
            {
                yield return this.GetWhitespaceToken();
            }
            else if (Keyword.MatchSeparator(this.reader, out var separator))
            {
                yield return this.GetSeparatorToken(separator!);
            }
            else
            {
                var position = this.reader.GetReadPosition();
                var value = this.GetString(this.IsValue);

#pragma warning disable IDE0046
                if (Keyword.MatchReserved(value, out var reserved))
                {
                    yield return new ReservedToken
                    {
                        Position = position,
                        Type = Enum.Parse<ReservedType>(reserved!.Name),
                        Value = value,
                    };
                }
                else if (Regex.IsMatch(value, @"^0x[a-fA-F0-9]+$"))
                {
                    yield return new HexadecimalToken
                    {
                        Position = position,
                        Value = value,
                    };
                }
                else if (Regex.IsMatch(value, @"^0[0-7]+$"))
                {
                    yield return new OctalToken
                    {
                        Position = position,
                        Value = value,
                    };
                }
                else if (Regex.IsMatch(value, @"^-?[0-9]+$"))
                {
                    yield return new DecimalToken
                    {
                        Position = position,
                        Value = value,
                    };
                }
                else
                {
                    yield return Regex.IsMatch(value, @"^\*?[a-zA-Z][a-zA-Z0-9_]*$")
                        ? new IdentifierToken
                        {
                            Position = position,
                            Value = value,
                        }
                        : new UnknownToken
                        {
                            Position = position,
                            Value = value,
                        };
                }
#pragma warning restore IDE0046
            }
        }
    }

    private CommentToken GetCommentBlockToken(Position position)
    {
        var value = new StringBuilder();

        while (!this.reader.StartsWith(CommentEnd))
        {
            if (this.reader.EndOfStream)
            {
                throw new Exception($"Close comment starting at {position}.");
            }

            var ch = this.reader.Read();
            if (ch != EOS)
            {
                _ = value.Append((char)ch);
            }
        }

        this.reader.Skip(CommentEnd.Length);

        return new CommentToken
        {
            Position = position,
            Value = value.ToString(),
        };
    }

    private CommentToken GetCommentLineToken(Position position)
    {
        var value = new StringBuilder();

        while (!this.reader.StartsWith(this.IsNewline))
        {
            if (this.reader.EndOfStream)
            {
                break;
            }

            var ch = this.reader.Read();
            if (ch != EOS)
            {
                _ = value.Append((char)ch);
            }
        }

        return new CommentToken
        {
            Position = position,
            Value = value.ToString(),
        };
    }

    private SeparatorToken GetSeparatorToken(Keyword keyword)
    {
        var position = this.reader.GetReadPosition();

        var value = new StringBuilder();
        _ = value.Append((char)this.reader.Read());

        return new SeparatorToken
        {
            Position = position,
            Type = Enum.Parse<SeparatorType>(keyword.Name),
            Value = value.ToString(),
        };
    }

    private string GetString(Predicate<int> matcher)
    {
        var value = new StringBuilder();

        while (this.reader.StartsWith(matcher))
        {
            if (this.reader.EndOfStream)
            {
                break;
            }

            var ch = this.reader.Read();
            if (ch != EOS)
            {
                _ = value.Append((char)ch);
            }
        }

        return value.ToString();
    }

    private WhitespaceToken GetWhitespaceToken()
    {
        return new WhitespaceToken
        {
            Position = this.reader.GetReadPosition(),
            Value = this.GetString(this.IsWhitespace),
        };
    }

    private bool IsNewline(int value)
    {
        return value == '\n';
    }

    private static bool IsSeparator(int value)
    {
        return Keyword.SeparatorChars.Contains(value);
    }

    private bool IsWhitespace(int value)
    {
        return Keyword.WhiteSpaceChars.Contains(value);
    }

    private bool IsValue(int value)
    {
        // TODO: Add comment
        return value > -1 && !IsSeparator(value) && !this.IsWhitespace(value);
    }
}
