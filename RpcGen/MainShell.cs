namespace RpcGen;

internal class MainShell
{
    internal static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("rpcgen <PATH>");
            Environment.Exit(1);
        }

        var source = new FileInfo(args[0]);
        if (!source.Exists)
        {
            Console.Error.WriteLine("Not found `{0}`", args[0]);
            Environment.Exit(1);
        }

        var destination = new FileInfo(
            source.FullName.Replace(source.Extension, ".cs", StringComparison.InvariantCultureIgnoreCase));

        try
        {
            using var reader = source.OpenRead();
            using var writer = destination.OpenWrite();

            using var proto = new ProtoReader(reader);
            var lexer = new Lexer(proto);
            var parser = new Parser(lexer);
            var spec = parser.Parse();
            var gen = new Generator(spec);

            gen.Generate(writer, Utility.ToNsName(
                source.Name.Replace(source.Extension, string.Empty, StringComparison.InvariantCulture)));
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("{0}", e);
            Environment.Exit(1);
        }
    }
}
