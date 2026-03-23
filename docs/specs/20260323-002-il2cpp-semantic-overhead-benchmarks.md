# IL2CPP Semantic Overhead Benchmarks

## Metadata

### Source Plan / Request
User-requested implementation on 2026-03-23 to extend the synthetic benchmark MVP with IL2CPP hot-path semantic-overhead and micro-optimization experiment families.

### Status
done

### Related Specs
- `docs/specs/2026-03-22-001-benchmark-mvp.md`
- `docs/specs/20260323-001-collections-benchmark-expansion.md`

## Goal
Add a narrow synthetic benchmark family that makes hot-path semantic-overhead experiments directly runnable through the current Editor and Win64 IL2CPP Release Player workflow.

## In Scope
- add directly comparable synthetic cases for:
  - in-loop static access vs hoisted local
  - `const` scalar vs `static readonly` scalar
  - declaration-only static initialization vs explicit static constructor
  - helper chain vs flat helper
  - `params` helper vs fixed-arity helper
  - controlled custom static-holder access shape comparisons
- register all new cases explicitly in the synthetic catalog for the existing workload tiers
- keep measured paths narrow and use existing reporting surfaces
- add the minimum durable documentation needed to explain the new family and its interpretation limits

## Out of Scope
- framework or runner redesign
- reflection-based discovery
- new suites, dashboards, comparison engines, or result schemas
- real-world benchmark ingestion
- broad automation or Editor tooling
- performance claims beyond confirming the new cases run and export correctly

## Affected Area
- `Assets/UnityPerfLab/Runtime/Cases/Synthetic`
- `Assets/UnityPerfLab/README.md`
- `README.md`
- `docs/guides/*`
- `docs/specs/20260323-002-il2cpp-semantic-overhead-benchmarks.md`

## Task Checklist
- [x] Add a synthetic IL2CPP semantics benchmark case implementation with controlled paired variants across the planned hypothesis families.
- [x] Register the new cases explicitly for `1K`, `100K`, and `1M` workloads in the synthetic catalog.
- [x] Keep existing export workflow intact while ensuring descriptor naming is comparison-friendly in `summary.csv`, `overview.csv`, and `overview.md`.
- [x] Add focused durable docs covering the new benchmark families, run path, and interpretation limits for Editor vs IL2CPP Release Player results.
- [x] Validate that the synthetic suite still runs and exports the new cases through the existing workflow entry points.

## Done When
The synthetic suite contains the new semantic-overhead benchmark family with explicit registration, the existing export output can distinguish the new variants without schema changes, and the benchmark module docs explain what the new cases do and how to interpret them.

## Validation

### Black-box Checks
- Run the existing synthetic suite entry point and confirm exported results include the new benchmark cases.
- Run the existing Win64 IL2CPP Release Player validation path if available in this environment and confirm the exported results include the same new cases.
- Check `summary.csv`, `overview.csv`, and `overview.md` for readable grouping of the new benchmark variants without changing the primary CSV schema.

### White-box Needed
No

### White-box Trigger
This change adds controlled synthetic cases and explicit registration rather than modifying branch-heavy runner logic. Reviewable acceptance is based on successful execution and exported outputs.

### Internal Logic To Protect
N/A

## Write-back Needed
Yes

Write back only stable, reusable benchmark-family guidance to module documentation and a focused guide for interpreting these synthetic IL2CPP-oriented cases.

## Risks / Notes
- These cases are synthetic observations of code-shape differences and should not claim undocumented engine internals.
- Editor runs remain smoke validation only; IL2CPP Release Player results are the real decision signal for later optimization follow-up.
- Descriptor names and parameters need to stay explicit enough that exported rows remain interpretable without additional tooling.
- Editor batchmode validation passed on 2026-03-23 with Unity `2022.3.16f1` and exported all 36 new `SemanticOverhead` rows.
- Win64 IL2CPP Release batchmode build and player autorun validation both passed on 2026-03-23 with Unity `2022.3.16f1`, and the Player exported all 36 new `SemanticOverhead` rows with `platform = WindowsPlayer` and `scriptingBackend = IL2CPP`.
