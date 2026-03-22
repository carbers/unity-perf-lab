# Specs Index

This directory contains specification documents for `unity-perf-lab`.

## Naming Rule

SPEC files use:

- `YYYY-MM-DD-NNN-topic.md`

Examples:

- `2026-03-22-001-benchmark-mvp.md`
- `2026-04-05-002-collections-data-layout.md`
- `2026-03-22-901-mvp-parent-draft.md`

`NNN` is a stable sequence identifier, not a phase label. Active execution baselines should use the main working sequence such as `001`, `002`, `003`. Historical draft specs may use a reserved `9xx` range so they remain easy to distinguish without breaking the naming rule.

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
