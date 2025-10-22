namespace FileToolingLibrary;
public interface IFileResolver
{
    BasicList<string> ExtensionsAllowed { get; }
    Task<string> ResolveDataAsync(string path);
    string GetGlobalName { get; }
}