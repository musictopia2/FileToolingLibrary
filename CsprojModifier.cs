namespace FileToolingLibrary;
public static class CsprojModifier
{
    public static bool RemoveResourceEntries(string fileName)
    {
        if (!File.Exists(GlobalConstants.CsProjPath))
        {
            throw new FileNotFoundException("Project file not found.", GlobalConstants.CsProjPath);
        }
        XDocument doc = XDocument.Load(GlobalConstants.CsProjPath);
        string resourcePath = $"Resources\\{fileName}";
        bool changed = false;

        // Find all <None Remove="Resources\fileName" />
        var noneItems = doc.Descendants()
            .Where(x => x.Name.LocalName == "None" &&
                        (string)x.Attribute("Remove")! == resourcePath)
            .ToList();

        // Find all <EmbeddedResource Include="Resources\fileName" />
        var embeddedItems = doc.Descendants()
            .Where(x => x.Name.LocalName == "EmbeddedResource" &&
                        (string)x.Attribute("Include")! == resourcePath)
            .ToList();

        var itemsToRemove = noneItems.Concat(embeddedItems).ToList();

        foreach (var item in itemsToRemove)
        {
            var parent = item.Parent;
            item.Remove();
            changed = true;

            // If the parent <ItemGroup> is now empty, remove it
            if (parent != null && !parent.Elements().Any())
            {
                parent.Remove();
            }
        }

        if (changed)
        {
            doc.Save(GlobalConstants.CsProjPath);
        }

        return changed;
    }
}