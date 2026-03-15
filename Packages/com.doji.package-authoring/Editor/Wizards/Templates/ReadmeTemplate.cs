namespace Doji.PackageAuthoring.Editor.Wizards.Templates {
    /// <summary>
    /// Builds README files for the generated package repository.
    /// </summary>
    public static class ReadmeTemplate {
        public static string GetPackageReadme(PackageContext ctx) {
            return $"# {ctx.Package.PackageName}\n\n{ctx.Package.Description}";
        }

        public static string GetRepositoryReadme(PackageContext ctx) {
            return $@"<a href=""https://www.doji-tech.com/"">
  <img src=""https://www.doji-tech.com/favicon.ico"" alt=""doji logo"" title=""Doji"" align=""right"" height=""70"" />
</a>

# {ctx.Project.ProductName}
{ctx.Package.Description}

[OpenUPM] - [Documentation (coming soon)]

[OpenUPM]: https://openupm.com/packages/{ctx.Package.PackageName}
[Documentation (coming soon)]: https://github.com/julienkay/{ctx.Package.PackageName}
";
        }
    }
}
