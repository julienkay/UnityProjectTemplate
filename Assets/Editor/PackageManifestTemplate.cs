using System.Collections.Generic;

public partial class PackageScaffolder{
    public string GetPackageManifest() {
        // Format the dependencies as JSON
        var dependencyLines = new List<string>();
        foreach (var dep in dependencies) {
            dependencyLines.Add($"    \"{dep.packageName}\": \"{dep.version}\"");
        }
        string dependenciesJson = dependencyLines.Count > 0 ? $"\"dependencies\": {{\n{string.Join(",\n", dependencyLines)}\n  }}" : "";

        return $@"{{
  ""name"": ""{packageName}"",
  ""version"": ""{version}"",
  ""displayName"": ""{productName}"",
  ""description"": ""{description}"",
  {dependenciesJson},
  ""author"": {{
    ""name"": ""Doji Technologies"",
    ""email"": ""support@doji-tech.com"",
    ""url"": ""https://www.doji-tech.com/""
  }}
}}";
    }
}
