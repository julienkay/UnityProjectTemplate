namespace Doji.PackageAuthoring.Editor {
    public partial class PackageCreationWizard {
        public string GetPackageReadme() {
            return $"# {_packageName}\n\n{_description}";
        }

        public string GetRepositoryReadme() {
            return $@"<a href=""https://www.doji-tech.com/"">
  <img src=""https://www.doji-tech.com/favicon.ico"" alt=""doji logo"" title=""Doji"" align=""right"" height=""70"" />
</a>

# {_productName}
{_description}

[OpenUPM] - [Documentation (coming soon)]

[OpenUPM]: https://openupm.com/packages/{_packageName}
[Documentation (coming soon)]: https://github.com/julienkay/{_packageName}
";
        }
    }
}
