namespace Doji.PackageAuthoring.Editor {
    public partial class PackageCreationWizard {
        public string GetDocfxJson() {
            return $@"{{
  ""metadata"": [
    {{
      ""src"": [
        {{
          ""files"": [
            ""**/*.cs""
          ],
          ""src"": ""../{_packageName}""
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

        public string GetDocfxPdfJson() {
            return $@"{{
  ""metadata"": [
    {{
      ""src"": [
        {{ ""files"": [ ""{_assemblyName.ToLower()}.csproj"" ], ""src"": ""../projects/{_productName}"" }}
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
      ""pdfFileName"": ""{_packageName}.pdf""
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

        public string GetFilterConfig() {
            string ns = _namespaceName.Replace(".", @"\.");
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

        public string GetIndexMD() {
            return $@"# {_productName}

{_description}.";
        }

        public string GetRootToc() {
            return $@"- name: Manual
  href: manual/
- name: Scripting API
  href: api/
  homepage: api/index.md
";
        }

        public string GetManualToc() {
            return $@"- name: {_productName}
  href: ../index.md
";
        }
    }
}
