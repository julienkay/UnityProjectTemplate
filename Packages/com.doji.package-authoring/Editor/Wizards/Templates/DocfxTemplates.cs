namespace Doji.PackageAuthoring.Editor.Wizards.Templates {
    /// <summary>
    /// Builds docfx configuration files for generated package documentation.
    /// </summary>
    public static class DocfxTemplates {
        public static string GetDocfxJson(PackageContext ctx) {
            return $@"{{
  ""metadata"": [
    {{
      ""src"": [
        {{
          ""files"": [
            ""**/*.cs""
          ],
          ""src"": ""../{ctx.Package.PackageName}""
        }}
      ],
      ""dest"": ""api"",
      ""includePrivateMembers"": false,
      ""disableGitFeatures"": false,
      ""disableDefaultFilter"": false,
      ""noRestore"": false,
      ""namespaceLayout"": ""flattened"",
      ""memberLayout"": ""samePage"",
      ""EnumSortOrder"": ""declaringOrder"",
      ""allowCompilationErrors"": true,
      ""globalNamespaceId"": ""Global"",
      ""filter"": ""filterConfig.yml""
    }}
  ],
  ""build"": {{
    ""content"": [
      {{
        ""files"": [
          ""api/**.yml"",
          ""api/index.md""
        ]
      }},
      {{
        ""files"": [
          ""manual/**.md"",
          ""manual/**/toc.yml"",
          ""toc.yml"",
          ""*.md""
        ]
      }}
    ],
    ""resource"": [
      {{
        ""files"": [
          ""images/**"",
        ]
      }}
    ],
    ""output"": ""_site"",
    ""globalMetadataFiles"": [],
    ""globalMetadata"": {{
      ""_appFaviconPath"": ""images/favicon.ico"",
      ""_appLogoPath"": ""images/doji.png"",
      ""_appLogoUrl"": ""https://docs.doji-tech.com/"",
      ""_disableContribution"": true
    }},
    ""fileMetadataFiles"": [],
    ""template"": [
      ""default""
    ],
    ""postProcessors"": [],
    ""keepFileLink"": false,
    ""disableGitFeatures"": false
  }}
}}";
        }

        public static string GetDocfxPdfJson(PackageContext ctx) {
            return $@"{{
  ""metadata"": [
    {{
      ""src"": [
        {{ ""files"": [ ""{ctx.Package.AssemblyName.ToLower()}.csproj"" ], ""src"": ""../projects/{ctx.Project.ProductName}"" }}
      ],
      ""dest"": ""api"",
      ""includePrivateMembers"": false,
      ""disableGitFeatures"": false,
      ""disableDefaultFilter"": false,
      ""noRestore"": false,
      ""namespaceLayout"": ""flattened"",
      ""memberLayout"": ""samePage"",
      ""EnumSortOrder"": ""declaringOrder"",
      ""allowCompilationErrors"": false,
      ""globalNamespaceId"": ""Global"",
      ""filter"": ""filterConfig.yml""
    }}
  ],
  ""build"": {{
    ""content"": [
      {{
        ""files"": [
          ""api/**.yml"",
          ""api/index.md""
        ]
      }},
      {{
        ""files"": [
          ""manual/**.md"",
          ""manual/**/toc.yml"",
          ""toc.yml"",
          ""*.md""
        ]
      }},
      {{ ""files"": [ ""pdf/toc.yml"" ] }}
    ],
    ""resource"": [
      {{
        ""files"": [
          ""images/**"",
        ]
      }}
    ],
    ""output"": ""_site"",
    ""globalMetadataFiles"": [],
    ""globalMetadata"": {{
      ""_appFaviconPath"": ""images/favicon.ico"",
      ""_appLogoPath"": ""images/doji.png"",
      ""_appLogoUrl"": ""https://www.doji-tech.com/"",
      ""_disableContribution"": true,
      ""pdf"": true,
      ""pdfTocPage"": true,
      ""pdfFileName"": ""{ctx.Package.PackageName}.pdf""
    }},
    ""fileMetadataFiles"": [],
    ""template"": [
      ""default"",
      ""modern"",
      ""template""
    ],
    ""postProcessors"": [],
    ""keepFileLink"": false,
    ""disableGitFeatures"": false
  }}
}}";
        }

        public static string GetFilterConfig(PackageContext ctx) {
            string ns = ctx.Package.NamespaceName.Replace(".", @"\.");
            return $@"apiRules:
- include: # The namespaces to generate
    uidRegex: ^{ns}
    type: Namespace
- include:
    uidRegex: ^Global
    type: Namespace
- exclude:
    uidRegex: ^{ns}\.Editor
- exclude:
    uidRegex: ^{ns}\.Samples";
        }

        public static string GetIndexMD(PackageContext ctx) {
            return $@"# {ctx.Project.ProductName}

{ctx.Package.Description}.";
        }

        public static string GetRootToc(PackageContext ctx) {
            return $@"- name: Manual
  href: manual/
- name: Scripting API
  href: api/
  homepage: api/index.md
";
        }

        public static string GetManualToc(PackageContext ctx) {
            return $@"- name: {ctx.Project.ProductName}
  href: ../index.md
";
        }
    }
}
