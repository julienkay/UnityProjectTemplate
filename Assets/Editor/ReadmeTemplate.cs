public partial class PackageScaffolder{
    public string GetPackageReadme() {
        return $"# {packageName}\n\n{description}";
    }
    public string GetRepositoryReadme() {
        return $@"<a href=""https://www.doji-tech.com/"">
  <img src=""https://www.doji-tech.com/assets/favicon.ico"" alt=""doji logo"" title=""Doji"" align=""right"" height=""70"" />
</a>

# {productName}
{description}

[OpenUPM] · [Documentation (coming soon)]

[OpenUPM]: https://openupm.com/packages/{packageName}
[Documentation (coming soon)]: https://github.com/julienkay/{packageName}
";
    }
}
