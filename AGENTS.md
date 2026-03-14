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

## C# Style (Unity)

- No raw public fields.
- Prefer properties over public fields.
- Use `[field: SerializeField] public TYPE Name { get; private set; }` when inspector/serializer input should be externally read-only.
- Use `[field: SerializeField] public TYPE Name { get; set; }` for serialized data objects/DTOs that are intentionally edited through tooling code.
- Use `[SerializeField] private TYPE name;` for private Inspector fields.
- Private instance fields use `_camelCase`.
- Private computed properties may use `PascalCase` when they read like derived values rather than stored state.
- Private static readonly shared collections/values may use `PascalCase` when treated as internal constants/shared state.
- Place multiple attributes on separate lines.
- Prefer property-based public APIs; keep backing fields private or compiler-generated.
- Follow Unity conventions for `MonoBehaviour` and `EditorWindow` methods; do not add unnecessary `Update()` or `FixedUpdate()`.

## Documentation Style

- Document every public type and any non-obvious internal type with an XML summary.
- Start summaries high-level: explain role and intent before mechanics.
- Do not narrate obvious code. Avoid filler like `Gets or sets`, `Helper for`, or comments that restate the method name.
- Add detail only where behavior is surprising, constrained by Unity/serialization, or shaped by a workaround.
- Prefer short summaries plus targeted `<param>` notes over long blocks.
- Document why a piece exists or what contract it preserves when that is not obvious from the implementation.

## Avoid

- Do not modify `Library/`, `Temp/`, `Obj/`, `Logs/`, .idea` or `UserSettings` unless the task is explicitly about them.

## Validation

For structural or tooling changes, verify as much as practical by checking:

- asmdef references and root namespaces
- that editor-only packages do not depend on unnecessary runtime assemblies
- generated manifests, asmdefs, and template output when relevant
- that Unity-specific moves preserve matching `.meta` files

If Unity is not run, say validation was limited to source inspection.
