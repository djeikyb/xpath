using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace xpath;

internal static class Program
{
    public static int Main(string[] rawArgs)
    {
        if (rawArgs.Length < 2)
        {
            return Die("Wrong number of cli args.");
        }

        var xpath = rawArgs[0];
        Console.Error.WriteLine($"xpath: {xpath}");
        Console.Error.WriteLine($"xml files:\n{string.Join('\n', rawArgs[1..])}");

        var stdout = Console.OpenStandardOutput();

        var resolver = new XmlNamespaceManager(new NameTable());
        resolver.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
        resolver.AddNamespace("etm", "http://www.smpte-ra.org/schemas/430-3/2006/ETM");
        resolver.AddNamespace("kdm", "http://www.smpte-ra.org/schemas/430-1/2006/KDM");
        resolver.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
        resolver.AddNamespace("gr", "http://www.smpte-ra.org/ns/395/2016");
        resolver.AddNamespace("cpl", "http://www.smpte-ra.org/schemas/429-7/2006/CPL");
        resolver.AddNamespace("am", "http://www.smpte-ra.org/schemas/429-9/2007/AM");

        foreach (var file in rawArgs[1..])
        {
            using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            var doc = XDocument.Load(fs);

            if (doc.Root == null)
            {
                continue;
            }

            resolver.AddNamespace("r", doc.Root.GetDefaultNamespace().NamespaceName);

            var found = doc.XPathEvaluate(xpath, resolver);

            if (found is IEnumerable iter)
            {
                foreach (var item in iter)
                {
                    var xtext = item.ToString() ??
                                throw new Exception($"{item.GetType()} representation was null.");
                    stdout.Write(Utf8.GetBytes(xtext));
                    stdout.Write("\n"u8);
                }
            }
            else
            {
                var scalar = found.ToString() ??
                             throw new Exception($"{found.GetType()} representation was null.");
                stdout.Write(Utf8.GetBytes(scalar));
                stdout.Write("\n"u8);
            }
        }

        return 0;
    }

    private static readonly UTF8Encoding Utf8 = new(encoderShouldEmitUTF8Identifier: false);

    public static int Die(string msg)
    {
        Console.WriteLine(msg);
        return -1;
    }
}
