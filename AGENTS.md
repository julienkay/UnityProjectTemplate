# AGENTS.md

## Project Intent

This repository is a Unity 6.3 template project centered on editor tooling for creating reusable Unity packages and companion sample or test projects.

## Source Of Truth

Prefer these locations:

- `Packages/`: local packages under development
- `Assets/`: project assets and editor tooling that support the template project
- `Assets/Input` and `Assets/Settings`: baseline assets intended to be copied into generated projects
- `ProjectSettings` and `Packages/manifest.json`: default Unity project configuration for generated outputs

Prefer working in existing package folders when functionality is package-owned.

## Conventions

When adding code:

- keep editor-only code in `Editor` folders or editor-only assemblies
- use asmdefs deliberately and keep assembly references explicit
- keep namespaces, folders, and asmdef boundaries coherent for Unity and Rider
- preserve `.meta` files when moving Unity assets or scripts

## Avoid

Do not edit these unless the task is explicitly about them:

- `Library`
- `Temp`
- `Logs`
- `obj`
- `.idea`
- `UserSettings`

## Validation

For structural or tooling changes, verify as much as practical by checking:

- asmdef references and root namespaces
- that editor-only packages do not depend on unnecessary runtime assemblies
- generated manifests, asmdefs, and template output when relevant
- that Unity-specific moves preserve matching `.meta` files

If Unity is not run, say validation was limited to source inspection.
