public partial class PackageScaffolder {
    public string GetDocfxJson() {
        return $@"{{
  ""metadata"": [
    {{
      ""src"": [
        {{
          ""files"": [
            ""**/*.cs""
          ],
          ""src"": ""../{packageName}""
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
      ""_appLogoUrl"": ""https://www.doji-tech.com/"",
      ""_disableContribution"": true
    }},
    ""fileMetadataFiles"": [],
    ""template"": [
      ""default"",
      ""templates/unity""
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
        {{ ""files"": [ ""{assemblyName.ToLower()}.csproj"" ], ""src"": ""../projects/{productName}"" }}
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
      ""pdfFileName"": ""{packageName}.pdf""
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
        string ns = namespaceName.Replace(".", @"\.");
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
        return $@"# {productName}

{description}.";
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
        return $@"- name: {productName}
  href: ../index.md
";
    }
}
