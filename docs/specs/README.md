# Specs Directory

Task specs are the default execution artifact between planning and implementation in `unity-perf-lab`.

## Purpose

Use `docs/specs/*` to hold narrow task specs that:
- bridge plans and implementation
- keep execution reviewable
- stay small enough to refine during iteration

This directory usually does not need a central index.
This repository keeps a lightweight baseline and legacy-draft index below because the project already has active and historical spec artifacts worth preserving.

## Naming

Store new specs as:

`docs/specs/YYYYMMDD-NNN-task-slug.md`

- `YYYYMMDD` is the spec creation date
- `NNN` is the same-day sequence, starting at `001`
- choose the next available same-day sequence by scanning existing spec filenames that already use this format
- never renumber existing specs
- `task-slug` is lowercase kebab-case

Example:

`docs/specs/20260322-001-auth-session-hardening.md`

## Legacy Specs

The existing `2026-03-22-*.md` files use an earlier `YYYY-MM-DD-NNN-task-slug.md` format.
Keep those files and their references unchanged.
Do not rename legacy specs as part of routine SOP maintenance.

Legacy `9xx` draft IDs remain historical markers for earlier planning slices.
When creating new active specs, continue using the normal same-day sequence such as `001`, `002`, `003`.
Do not treat legacy `9xx` drafts as the next active sequence number.

## Create Or Refine

- if a plan or phase slice exists, derive one or more specs before editing
- if iterating within the same reviewable slice, refine the existing spec
- if the primary outcome, boundary, or validation path changes, create a new dated spec first
- only tiny task requests that are already effectively spec-complete and trivially narrow may skip spec creation

## Keep Specs Small

One spec should produce one primary reviewable outcome.
One plan may produce multiple specs.

Split a spec when it would contain:
- multiple primary outcomes
- independently reviewable slices
- distinct validation paths
- a checklist that no longer stays short

## Status

Use one of these status values:

- `draft`: the spec exists, but implementation should not start yet
- `in-progress`: implementation for this spec has started
- `blocked`: progress is waiting on a prerequisite, dependency, or decision
- `done`: `Done When` is satisfied and required validation has passed

Checklist completion alone does not make a spec `done`.
Required validation must also pass.

## Checklist

Use a short Markdown checkbox list inside each spec.

- keep it narrow, usually 3-7 items
- make items concrete and outcome-oriented
- do not use the checklist as a work log
- do not use the checklist as a full project board

## Current Execution Baseline

- `2026-03-22-001-benchmark-mvp.md`
  Canonical Phase 1 SPEC for the `UnityPerfLab` synthetic benchmark MVP.

## Superseded Drafts

The following earlier MVP planning documents remain in the repository as historical drafts and context, but they are not the current execution baseline:

- `2026-03-22-901-mvp-parent-draft.md`
- `2026-03-22-902-bootstrap-boundaries-draft.md`
- `2026-03-22-903-runner-reporting-core-draft.md`
- `2026-03-22-904-synthetic-suite-editor-entry-draft.md`
- `2026-03-22-905-player-build-module-docs-draft.md`

When implementation changes stable project behavior or boundaries, write the lasting outcome to the correct layer instead of expanding draft specs.
