public static class PackageManifestTemplate {
    public static string GetContent(string productName, string packageName, string description, string companyName, string version) {
        return $@"{{
  ""name"": ""{packageName}"",
  ""version"": ""{version}"",
  ""displayName"": ""{productName}"",
  ""description"": ""{description}"",
  ""dependencies"": {{
    ""com.unity.sentis"": ""2.0.0""
  }},
  ""author"": {{
    ""name"": ""Doji Technologies"",
    ""email"": ""support@doji-tech.com"",
    ""url"": ""https://www.doji-tech.com/""
  }}
}}";
    }
}
