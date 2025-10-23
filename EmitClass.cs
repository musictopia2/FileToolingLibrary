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


    public static string ToValidIdentifier(string className, bool toLower)
    {

        bool rets = int.TryParse(className, out int value);
        if (rets)
        {
            if (toLower)
            {
                return $"file{value}";
            }
            else
            {
                return $"File{value}";
            }
        }

        // Remove any non-letter/digit characters (like -, _, spaces, etc.)
        string otherName = CSharpRegEx().Replace(className, "");
        string digits = DigitsRexEx().Match(className).Value;
        // Keep trailing digits but remove any digits that appear before the end.
        // This uses a lookahead to only match digits that are *not* followed by the end of the string.
        otherName = DigitRemoveRegEx().Replace(otherName, ""); // remove digits followed by a letter
        otherName = CSharpRegEx().Replace(otherName, "");      // remove digits at the start
        return otherName;
    }

    private static void PopulateDetails(ICodeBlock w, FileClass item)
    {
        var safeData = ToLiteral(item.Data); // escapes string for C# literal
        string safeName1 = ToValidIdentifier(item.ClassName.ToLower(), true);
        string safeName2 = ToValidIdentifier(item.ClassName.CapitalizeFirstLetter(), false);
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
                        string safeName = ToValidIdentifier(item.ClassName.ToLower(), true);
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
    [GeneratedRegex(@"\d+")]
    private static partial Regex DigitsRexEx();
    [GeneratedRegex(@"\d+(?=\D)")]
    private static partial Regex DigitRemoveRegEx();
}