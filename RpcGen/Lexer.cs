namespace RpcGen;

using System.Text;
using System.Text.RegularExpressions;

public class Lexer
{
    private const int EOS = -1;

    private const string CommentStart = "/*";
    private const string CommentEnd = "*/";
    private const string CommentLine = "%";

    private readonly ProtoReader reader;

    public Lexer(ProtoReader reader)
    {
        this.reader = reader;
    }

    public IEnumerable<Token> Enumerate()
    {
        while (this.reader.Peek() != -1)
        {
            if (this.reader.StartsWith(Lexer.CommentStart))
            {
                var position = this.reader.GetReadPosition();
                this.reader.Skip(Lexer.CommentStart.Length);
                yield return this.GetCommentBlockToken(position);
            }
            else if (this.reader.StartsWith(Lexer.CommentLine))
            {
                var position = this.reader.GetReadPosition();
                this.reader.Skip(Lexer.CommentLine.Length);
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
                else if (Regex.IsMatch(value, @"^\*?[a-zA-Z][a-zA-Z0-9_]*$"))
                {
                    yield return new IdentifierToken
                    {
                        Position = position,
                        Value = value,
                    };
                }
                else
                {
                    yield return new UnknownToken
                    {
                        Position = position,
                        Value = value,
                    };
                }
            }
        }
    }

    private CommentToken GetCommentBlockToken(Position position)
    {
        var value = new StringBuilder();

        while (!this.reader.StartsWith(Lexer.CommentEnd))
        {
            if (this.reader.EndOfStream)
            {
                throw new Exception($"Close comment starting at {position}.");
            }

            var ch = this.reader.Read();
            if (ch != Lexer.EOS)
            {
                value.Append((char)ch);
            }
        }

        this.reader.Skip(Lexer.CommentEnd.Length);

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
            if (ch != Lexer.EOS)
            {
                value.Append((char)ch);
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
        value.Append((char)this.reader.Read());

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
            if (ch != Lexer.EOS)
            {
                value.Append((char)ch);
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

    private bool IsSeparator(int value)
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
        return value > -1 && !this.IsSeparator(value) && !this.IsWhitespace(value);
    }
}
