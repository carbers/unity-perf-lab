# unity-perf-lab

`unity-perf-lab` is a Unity project dedicated to maintainable performance experiment work. Its primary product-facing module is `Assets/UnityPerfLab`, which provides a small benchmark infrastructure for Unity / IL2CPP micro-optimization research.

This repository still keeps the SOP layer that originally seeded the project, so the repo now serves two roles at once:

1. a Unity project root for benchmark execution
2. a lightweight SOP workspace for plan-first engineering, task specs, validation, and selective write-back

## What UnityPerfLab Is For

`UnityPerfLab` is intended to support repeatable performance experiments such as:

1. `foreach` vs `for`
2. collection and container implementation comparisons
3. direct call vs interface or virtual dispatch cost
4. boxing, generics, and abstraction overhead
5. closure, lambda, LINQ, and allocation-related hypotheses

The first MVP focuses on a working closed loop:

1. explicit benchmark case registration
2. runtime execution through a unified runner
3. CSV result export
4. Editor-side run entry points
5. Win64 IL2CPP Release scripted build
6. Player autorun through a bootstrap scene

## Repository Layout

- `Assets/UnityPerfLab/`
  Benchmark module root. See [Assets/UnityPerfLab/README.md](Assets/UnityPerfLab/README.md) for module-specific usage.
- `Packages/`
  Unity package manifest and package configuration.
- `ProjectSettings/`
  Unity project settings and build settings.
- `docs/guides/*`
  SOP usage guidance that still applies to work done in this repo.
- `docs/templates/*`
  Reusable plan, task spec, change-summary, and prompt scaffold templates.
- `docs/specs/*`
  Task specs used as the default execution artifact between planning and implementation.
- `docs/facts/*`
  Stable project context and validation-relevant facts.
- `skills/*`
  Reusable workflow skills such as plan-to-spec conversion.

## Working Model

The expected workflow for changes in this repo is still:

1. start from a plan, a phase slice, or a clearly scoped request
2. derive or refine one or more narrow task specs
3. implement the smallest coherent slice
4. validate explicitly
5. write back only stable, reusable facts
6. promote repeated workflows into skills when they stabilize

When a plan or phase slice exists, the default execution path is `plan -> one or more task specs -> implementation -> validation`.
A written plan document is optional. Use `docs/templates/plan-template.md` only when the plan should become a durable repo artifact worth re-reading, sharing, or handing off.
Plans may remain temporary. The task spec is the default durable execution artifact for implementation and iteration.
If iterating within the same reviewable slice, refine the existing spec. If the primary outcome, boundary, or validation path changes, create a new dated spec first.
Only tiny task requests that are already effectively spec-complete and trivially narrow may skip spec creation.

Black-box validation remains the default acceptance path. White-box validation is added when internal logic is branch-heavy, stateful, contract-sensitive, or otherwise regression-prone.

## Current Benchmark Module

The current `UnityPerfLab` MVP includes:

1. isolated asmdef layering
2. a unified benchmark runner and case abstraction
3. synthetic benchmark suites with explicit workload sizes across looping, collections, dispatch, and closure families
4. CSV export to `summary.csv` and `raw_samples.csv`
5. environment metadata export
6. Editor menu entry points
7. Win64 IL2CPP Release build entry

Real-world adapter hooks are scaffolded, but no real project-specific runtime case is wired yet because the repository does not currently contain reusable business/runtime implementations.

## Where To Look Next

1. read [Assets/UnityPerfLab/README.md](Assets/UnityPerfLab/README.md) for benchmark-module usage
2. read [AGENTS.md](AGENTS.md) for repository operating rules
3. read [docs/guides/new-project-sop.md](docs/guides/new-project-sop.md) for the repo workflow
4. read [docs/specs/2026-03-22-001-benchmark-mvp.md](docs/specs/2026-03-22-001-benchmark-mvp.md) for the current benchmark MVP contract, then [docs/specs/README.md](docs/specs/README.md) for the full spec index
5. if design, planning, and execution are split across tools or roles, use [docs/guides/design-to-spec-handoff.md](docs/guides/design-to-spec-handoff.md)
6. use [docs/templates/design-to-planner-prompt-template.md](docs/templates/design-to-planner-prompt-template.md) and [docs/templates/spec-to-executor-prompt-template.md](docs/templates/spec-to-executor-prompt-template.md) for structured handoff prompts
