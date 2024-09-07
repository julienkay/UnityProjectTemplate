public static class ReadmeTemplate {
    public static string GetPackageReadme(string packageName, string description) {
        return $"# {packageName}\n\n{description}";
    }
    public static string GetRepositoryReadme(string productName, string packageName, string description) {
        return $@"<a href=""https://www.doji-tech.com/"">
  <img src=""https://www.doji-tech.com/assets/favicon.ico"" alt=""doji logo"" title=""Doji"" align=""right"" height=""70"" />
</a>

# {productName}
{description}

[OpenUPM] · [Documentation (coming soon)] · [Feedback/Questions]

[OpenUPM]: https://openupm.com/packages/{packageName}
[Documentation (coming soon)]: https://github.com/julienkay/{packageName}
";
    }
}
