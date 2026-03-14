# Unity Project Template

This repository is a Unity 6 template project for bootstrapping either:

- a new Unity application project
- a reusable Unity package with an accompanying sample project

The repo is centered around custom editor tooling in [`Assets/Editor`](/Users/julienkipp/Unity/UnityProjectTemplate/Assets/Editor) rather than gameplay code. It provides a small baseline setup with URP assets, Input System defaults, and scaffolding utilities that generate new project/package structures from inside the Unity Editor.

## What This Repo Includes

- `ProjectCreationWizard`: creates a new Unity project by copying `Packages`, `ProjectSettings`, and the root `.gitignore`, while temporarily applying product/company/version settings.
- `PackageCreationWizard`: creates a UPM-style package repository with generated manifests, runtime assembly definitions, optional docs/tests/samples folders, and a local sample Unity project that references the package.
- Template helpers for common generated files such as `README.md`, `LICENSE`, `package.json`, `AssemblyInfo.cs`, DocFX config, and starter scripts.
- Default Unity project configuration for:
  - Universal Render Pipeline
  - Input System
  - common built-in Unity modules and IDE integration

Screenshot placeholder:
Add an inline image here showing the `Project Creation Wizard` and `Package Creation Wizard` windows, including the main metadata fields and package options.

## Unity Version

The project is currently configured for Unity `6000.3.5f1`.

## Repo Structure

Key folders:

- [`Assets/Editor`](/Users/julienkipp/Unity/UnityProjectTemplate/Assets/Editor): editor windows and text/template generators for project and package scaffolding
- [`Assets/Input`](/Users/julienkipp/Unity/UnityProjectTemplate/Assets/Input): baseline Input System actions asset
- [`Assets/Settings`](/Users/julienkipp/Unity/UnityProjectTemplate/Assets/Settings): URP renderer, pipeline, and volume profile assets
- [`Packages`](/Users/julienkipp/Unity/UnityProjectTemplate/Packages): Unity package manifest for the template project itself
- [`ProjectSettings`](/Users/julienkipp/Unity/UnityProjectTemplate/ProjectSettings): source project settings copied into generated projects
- [`docs`](/Users/julienkipp/Unity/UnityProjectTemplate/docs): placeholder docs directory for future documentation work

## How To Use

1. Open the repository in Unity `6000.3.5f1`.
2. Use the Unity menu:
   - `Tools/Project Creation Wizard`
   - `Tools/Package Creation Wizard`
3. Fill in the metadata fields and choose the target output location.
4. Run the wizard to generate the new project or package scaffold outside this template repo.

Screenshot placeholder:
Add an inline image here showing one wizard filled out with realistic example values, especially the output path, identifiers, and enabled optional folders.

## Generated Outputs

`ProjectCreationWizard` generates a fresh Unity project folder with copied package/project settings and project metadata applied.

`PackageCreationWizard` generates a repository shaped roughly like this:

```text
<root>/
  <package-name>/
    Runtime/
    package.json
  projects/
    <sample-project>/
  README.md
  LICENSE
  docs/   # optional
```

Optional package features include documentation, samples, editor-only code, and tests. The package wizard can also initialize a Git repository and add a GitHub remote.

Screenshot placeholder:
Add an inline image here showing the generated repository structure in Finder/Explorer or the Unity Project view, including the package folder and sample project.

## Default Dependencies

The template project currently includes:

- `com.unity.render-pipelines.universal`
- `com.unity.inputsystem`
- `com.unity.nuget.newtonsoft-json`
- `com.unity.ide.visualstudio`

Generated packages can also declare custom dependencies through the package wizard UI.

Screenshot placeholder:
Add an inline image here showing the baseline template setup in Unity, ideally `Assets/Input`, `Assets/Settings`, or the Package Manager with the main installed packages.

## Notes

- This repo appears to be a working template repository, not a polished distributable product yet.
- The generated package metadata and remote URLs currently contain Doji/GitHub-specific defaults, so you should expect to customize branding, author info, and repository links before wider reuse.
- The working tree currently contains local/editor state outside the template source itself, including `.idea` data and Unity-generated folders.
