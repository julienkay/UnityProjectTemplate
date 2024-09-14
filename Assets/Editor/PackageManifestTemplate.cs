using System.Collections.Generic;

public partial class PackageScaffolder{

    public string GetPackageManifest() {
        // Format the dependencies as JSON
        var dependencyLines = new List<string>();
        foreach (var dep in dependencies) {
            dependencyLines.Add($"    \"{dep.packageName}\": \"{dep.version}\"");
        }
        string dependenciesJson = dependencyLines.Count > 0 ? $"\"dependencies\": {{\n{string.Join(",\n", dependencyLines)}\n  }}," : "";
        string samplesJson = createSamplesFolder ? GetSamples() : "";

        return $@"{{
  ""name"": ""{packageName}"",
  ""version"": ""{version}"",
  ""displayName"": ""{productName}"",
  ""description"": ""{description}"",
  {dependenciesJson}
  ""author"": {{
    ""name"": ""Doji Technologies"",
    ""email"": ""support@doji-tech.com"",
    ""url"": ""https://www.doji-tech.com/""
  }},
  ""documentationUrl"": ""https://docs.doji-tech.com/{packageName}/""
  {samplesJson}
}}";
    }

    public string GetSamples() {
        return $@"""samples"": [
    {{
      ""displayName"": ""Shared Sample Assets (Required)"",
      ""description"": ""Shared resources for samples. All other samples depend on this being imported."",
      ""path"": ""Samples~/00-SharedSampleAssets""
    }},
    {{
      ""displayName"": ""Basic Sample"",
      ""description"": ""Basic example on how to use {productName}."",
      ""path"": ""Samples~/01-BasicSample""
    }}
  ],";
    }
}
