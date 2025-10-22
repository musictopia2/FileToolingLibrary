namespace FileToolingLibrary;
internal class EmitClass(BasicList<FileClass> list)
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
    private static void PopulateDetails(ICodeBlock w, FileClass item)
    {
        var safeData = ToLiteral(item.Data); // escapes string for C# literal

        w.WriteLine($"private readonly static string _{item.ClassName.ToLower()} = {safeData};")
         .WriteLine($"public static string {item.ClassName.CapitalizeFirstLetter()} => \"{item.FullName}\";");
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
                        w.WriteLine($"""
                            CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileContentRegistry.RegisterFile("{item.FullName}", _{item.ClassName.ToLower()});
                            """);
                    }
                });
        });
        string text = builder.ToString();
        ff1.WriteAllText(GlobalConstants.FinalName, text);
    }
}