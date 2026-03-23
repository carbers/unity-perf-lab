# IL2CPP Semantic Overhead Report

## Metadata

### Source Plan / Request
User-requested follow-up on 2026-03-23 to parse the latest Win64 IL2CPP Release Player benchmark results and produce a governance-oriented experiment report.

### Status
done

### Related Specs
- `docs/specs/20260323-002-il2cpp-semantic-overhead-benchmarks.md`
- `docs/specs/2026-03-22-001-benchmark-mvp.md`

## Goal
Turn the validated IL2CPP Release Player run into a focused experiment report that highlights the largest performance deltas, shows the compared code shapes, and records narrow governance guidance for future hot-path cleanup work.

## In Scope
- analyze the latest validated IL2CPP Release Player run for the semantic-overhead family
- compute pairwise differences using exported run data
- include a small number of code samples that match the measured shapes
- emphasize governance-relevant findings where the delta is materially large
- document what should be prioritized, what is neutral, and what still needs contextual validation

## Out of Scope
- new benchmark implementation
- rerunning unrelated benchmark families
- broad analytics tooling or report generation framework
- claiming universal rules beyond the validated synthetic patterns

## Affected Area
- `docs/specs/20260323-003-il2cpp-semantic-overhead-report.md`
- `docs/guides/*` or `docs/facts/*` as justified by the write-back decision

## Task Checklist
- [x] Compute paired IL2CPP player deltas for the semantic-overhead cases and identify the most material differences.
- [x] Draft a focused report with concrete result tables, code samples, and interpretation limits.
- [x] Record governance recommendations that are stable enough to reuse later.
- [x] Validate the report contents against the exported result files and the measured source code.

## Done When
The repository contains a reusable report that points future optimization work toward the largest validated hot-path semantic-overhead issues in the latest IL2CPP Release Player run without overstating what the synthetic data proves.

## Validation

### Black-box Checks
- Reconcile all reported numbers with the validated Player run under `Temp/PlayerValidationResults/...`.
- Confirm every code sample in the report matches the measured source shape in `Il2CppSemanticOverheadPerfCase.cs`.

### White-box Needed
No

### White-box Trigger
This task analyzes existing validated outputs and writes documentation. Acceptance depends on source/result consistency rather than new logic protection.

### Internal Logic To Protect
N/A

## Write-back Needed
Yes

Write back only the stable governance-facing report and keep raw execution detail anchored to the concrete validated run path instead of duplicating the full CSV.

## Risks / Notes
- The report should emphasize IL2CPP Release Player results, not Editor numbers.
- Governance recommendations should focus on clear high-delta patterns rather than low-signal noise.
- The report must state where a result is observational only and not enough to justify a blanket coding rule.
- Report write-back landed in `docs/facts/il2cpp-hot-path-governance.md`.
- Validation used the Win64 IL2CPP Release Player run at `Temp/PlayerValidationResults/20260323T144003790Z-il2cpp-release/20260323T064006621Z-981532/`.
