namespace RpcGen;

public class Position
{
    public static Position Empty { get; } = new Position();

    public int Column { get; set; }

    public int Line { get; set; }

    public override string ToString()
    {
        return $"{this.Line}:{this.Column}";
    }
}
