namespace FileToolingLibrary;
public class FileProcessor(IFileResolver resolver, params BasicList<string> args)
{
    public async Task ProcessAsync()
    {
        Console.WriteLine("Start Using File Tooling Library");
        if (args.Count != 3)
        {
            Console.WriteLine($"Expected 3 arguments but only received {args.Count}");
            return;
        }
        GlobalConstants.ProjectName = args[0];
        GlobalConstants.GlobalName = resolver.GetGlobalName;
        string projectDirectory = args[1];
        string projectFile = args[2];
        string resourcePath = Path.Combine(projectDirectory, "Resources");
        if (ff1.DirectoryExists(resourcePath) == false)
        {
            Console.WriteLine("There was no resources folder");
            return;
        }
        GlobalConstants.CsProjPath = Path.Combine(projectDirectory, projectFile);
        if (ff1.FileExists(GlobalConstants.CsProjPath) == false)
        {
            Console.WriteLine("There was no csproj file located");
            return;
        }
        BasicList<string> list = await ff1.FileListAsync(resourcePath);
        BasicList<FileClass> files = [];
        foreach (string item in list)
        {
            if (Path.GetExtension(item).Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                continue; //skip code files.
            }
            var ext = Path.GetExtension(item).ToLower();
            if (resolver.ExtensionsAllowed.Contains(ext) == false)
            {
                continue;
            }
            FileClass file = new();
            file.FullName = ff1.FullFile(item);
            file.ClassName = ff1.FileName(item);
            file.Data = await resolver.ResolveDataAsync(item);
            files.Add(file);
        }
        if (files.Count == 0)
        {
            Console.WriteLine("There was no files to convert over");
            return;
        }
        GlobalConstants.FinalName = Path.Combine(resourcePath, $"{GlobalConstants.GlobalName}.cs");
        EmitClass emits = new(files);
        emits.Emit();
        Console.WriteLine($"Created c# files for {GlobalConstants.GlobalName}.  Check out");
    }
}