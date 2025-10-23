using System.Globalization;
using System.Text.RegularExpressions;

namespace FileToolingLibrary;
internal partial class EmitClass(BasicList<FileClass> list)
{    
    public void Emit()
    {
        PopulateImages();
        RemoveResources();
    }
    private void RemoveResources()
    {
        foreach (var item in list)
        {
            CsprojModifier.RemoveResourceEntries(item.FullName);
        }
    }


    public static string ToValidIdentifier(string className)
    {
        // Remove any non-letter/digit characters (like -, _, spaces, etc.)
        string otherName = CSharpRegEx().Replace(className, "");

        // Keep trailing digits but remove any digits that appear before the end.
        // This uses a lookahead to only match digits that are *not* followed by the end of the string.
        otherName = Regex.Replace(otherName, @"\d+(?=\D)", ""); // remove digits followed by a letter
        otherName = Regex.Replace(otherName, @"^\d+", "");      // remove digits at the start

        return otherName;
    }

    private static void PopulateDetails(ICodeBlock w, FileClass item)
    {
        var safeData = ToLiteral(item.Data); // escapes string for C# literal
        string safeName1 = ToValidIdentifier(item.ClassName.ToLower());
        string safeName2 = ToValidIdentifier(item.ClassName.CapitalizeFirstLetter());
        w.WriteLine($"private readonly static string _{safeName1} = {safeData};")
         .WriteLine($"public static string {safeName2} => \"{item.FullName}\";");
    }
    public static string ToLiteral(string input)
    {
        return "\"" + input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n") + "\"";
    }
    private void PopulateImages()
    {
        SourceCodeStringBuilder builder = new();
        builder.WriteResourceClass(w =>
        {
            foreach (var item in list)
            {
                PopulateDetails(w, item);
            }
            w.WriteLine("public static void Register()")
                .WriteCodeBlock(w =>
                {
                    foreach (var item in list)
                    {
                        string safeName = ToValidIdentifier(item.ClassName.ToLower());
                        w.WriteLine($"""
                            CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileContentRegistry.RegisterFile("{item.FullName}", _{safeName});
                            """);
                    }
                });
        });
        string text = builder.ToString();
        ff1.WriteAllText(GlobalConstants.FinalName, text);
    }

    [GeneratedRegex(@"[^A-Za-z0-9]+")]
    private static partial Regex CSharpRegEx();
}