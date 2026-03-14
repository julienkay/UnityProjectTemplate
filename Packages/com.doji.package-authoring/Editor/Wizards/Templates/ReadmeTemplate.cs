namespace Doji.PackageAuthoring.Editor.Wizards {
    public partial class PackageCreationWizard {
        public string GetPackageReadme() {
            return $"# {_packageSettings.PackageName}\n\n{_packageSettings.Description}";
        }

        public string GetRepositoryReadme() {
            return $@"<a href=""https://www.doji-tech.com/"">
  <img src=""https://www.doji-tech.com/favicon.ico"" alt=""doji logo"" title=""Doji"" align=""right"" height=""70"" />
</a>

# {_projectSettings.ProductName}
{_packageSettings.Description}

[OpenUPM] - [Documentation (coming soon)]

[OpenUPM]: https://openupm.com/packages/{_packageSettings.PackageName}
[Documentation (coming soon)]: https://github.com/julienkay/{_packageSettings.PackageName}
";
        }
    }
}
