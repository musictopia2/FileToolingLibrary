namespace FileToolingLibrary;
internal static class SourceBuilderExtensions
{
    public static void WriteSimpleTypeClass(this SourceCodeStringBuilder builder, Action<ICodeBlock> action, FileClass file)
    {
        builder.WriteLine("#nullable enable")
                .WriteLine(w =>
                {
                    w.Write("namespace ")
                    .Write($"{GlobalConstants.ProjectName}.Resources")
                    .Write(";");
                })
                .WriteLine(w =>
                {
                    w.Write($"internal class {file.ClassName.CapitalizeFirstLetter()}");
                })
                .WriteCodeBlock(action.Invoke);
    }
    //future won't be text but doing now for testing.
    public static void WriteResourceClass(this SourceCodeStringBuilder builder, Action<ICodeBlock> action)
    {
        builder.WriteLine("#nullable enable")
                .WriteLine(w =>
                {
                    w.Write("namespace ")
                     .Write($"{GlobalConstants.ProjectName}.Resources")
                     .Write(";");
                })
                .WriteLine(w =>
                {
                    w.Write($"public static class {GlobalConstants.GlobalName}");
                })
                .WriteCodeBlock(action.Invoke);
    }
}