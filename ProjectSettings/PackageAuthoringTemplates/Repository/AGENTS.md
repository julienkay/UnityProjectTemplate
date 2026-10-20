# AGENTS.md

## Project Intent

This repository contains the Unity package `{{PACKAGE_NAME}}` and its companion project `{{PROJECT_NAME}}`.

{{PACKAGE_DESCRIPTION}}

Unity 6000.3 LTS. There is no compile step, CLI build command, or test runner to invoke. Do not attempt to build, compile, or run the project.

## Package Direction

- <replace this with custom, package-related instructions>

## Source Of Truth

Prefer these locations:

- repository root: orient from the repository root for searches, docs, and cross-package work; from this companion Unity project working directory, the repository root is `../..`
- `{{PACKAGE_NAME}}/`: local package under development
- `docs/`: user-facing package documentation only; keep maintainer notes and internal architecture docs out of DocFX content
- `.agents/index.md`: main entry point for internal maintainer docs when you need the broader doc map.
Keep durable project memory and maintainer-only planning and backlog notes under `.agents/docs`.
- `.agents/docs/current-work.md`: read this first for most sessions, active tasks, blockers, and next steps
- `.agents/docs/project-brief.md`: stable project intent, constraints, and repo context
- `.agents/docs/architecture/`: durable maintainer reference docs; read only the files relevant to the current task
- `.agents/docs/skills/`: project-specific skills; when a task matches one, read and follow the relevant `SKILL.md`
- `.agents/docs/project-memory.md`: durable project memory and agent-oriented project context
- `projects/{{PROJECT_NAME}}/Assets`: companion-project assets and test scenes

Prefer working in the package folder when functionality is package-owned.

## Conventions

When adding code:

- organize scripts by task area; prefer colocated or clearly scoped folders such as docs-, tooling-, or workflow-specific script locations over a shared miscellaneous scripts bucket
