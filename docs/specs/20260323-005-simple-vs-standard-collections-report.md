# Simple Vs Standard Collections Report

## Metadata

### Source Plan / Request
User-requested follow-up on 2026-03-23 to compare `PreUtil` simple collections against the standard .NET collections using the validated benchmark outputs and produce a focused report.

### Status
done

### Related Specs
- `docs/specs/20260323-004-custom-container-benchmark-expansion.md`
- `docs/specs/2026-03-22-001-benchmark-mvp.md`

## Goal
Turn the validated custom-container benchmark runs into a reusable report that highlights where `Simple*` containers are materially faster or slower than the standard collection baselines, while clearly separating apples-to-apples comparisons from lookup-shape observations.

## In Scope
- analyze the validated Editor and Win64 IL2CPP Release Player outputs for the custom-container cases
- compute direct pairwise comparisons for `LinkedList` vs `SimpleLinkList` traversal and add workloads
- compute direct pairwise comparisons for `Dictionary<int,int>` vs `SimpleIntDictionary<int>` lookup workloads
- summarize non-equivalent lookup-shape observations for `SimpleUIntDictionary`, `SimpleUlongDictionary`, and `SimpleStringDictionary`
- write back a focused report to `docs/facts/*` if the conclusions are stable enough to reuse

## Out of Scope
- rerunning benchmarks beyond the already validated runs unless required for consistency
- new benchmark implementation
- broad analytics tooling or permanent report-generation infrastructure
- claiming universal container rules outside the measured synthetic shapes

## Affected Area
- `docs/specs/20260323-005-simple-vs-standard-collections-report.md`
- `docs/facts/*` as justified by the write-back decision

## Task Checklist
- [x] Reconcile the validated Editor and Player result files used for the comparison report.
- [x] Compute direct simple-vs-standard deltas for the comparable collection pairs and identify the dominant findings.
- [x] Draft a focused report that separates strict comparisons from key-shape observations.
- [x] Validate all reported numbers against the exported result files and mark the spec complete.

## Done When
The repository contains a concise, reusable report that future readers can use to understand the current measured tradeoffs between `Simple*` and standard collections in this benchmark suite without overclaiming beyond the validated runs.

## Validation

### Black-box Checks
- Reconcile every reported number with the validated Editor run under `PerfLabResults/...` and the validated Player run under `H:\temp\unity-perf-lab-execute\PlayerResults/...`.
- Confirm every comparison labeled as direct uses equivalent workload, payload kind, and operation shape.

### White-box Needed
No

### White-box Trigger
This task analyzes existing outputs and writes documentation. Acceptance depends on report/result consistency rather than protecting new internal logic.

### Internal Logic To Protect
N/A

## Write-back Needed
Yes

Write back only the stable collection-comparison report and keep it tied to the concrete validated run paths instead of duplicating raw CSV output.

## Risks / Notes
- `SimpleUIntDictionary`, `SimpleUlongDictionary`, and `SimpleStringDictionary` do not share the exact same key shape as `Dictionary<int,int>` and should be reported separately from direct baseline comparisons.
- Editor numbers are useful as directional confirmation, but Win64 IL2CPP Release Player remains the primary basis for conclusions.
- Report write-back landed in `docs/facts/simple-vs-standard-collections.md`.
- Validation reconciled the report against:
  - `PerfLabResults/20260323T080527707Z-acf8fd/`
  - `H:\temp\unity-perf-lab-execute\PlayerResults\20260323T122906222Z-e25155/`
