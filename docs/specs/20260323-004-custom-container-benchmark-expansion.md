# Custom Container Benchmark Expansion

## Metadata

### Source Plan / Request
User-requested implementation on 2026-03-23 to extend the synthetic collections suite after landing the custom container package in-project, using `H:\DataStore\EffectiveTester` as a reference but keeping the current `UnityPerfLab` workflow and naming conventions.

### Status
done

### Related Specs
- `docs/specs/2026-03-22-001-benchmark-mvp.md`
- `docs/specs/20260323-001-collections-benchmark-expansion.md`

## Goal
Expand the synthetic collections suite with custom-container traversal and lookup cases so the current project can compare built-in and `PreUtil` container behavior without importing the legacy tester framework.

## In Scope
- add `SimpleLinkList<int|class|struct>` traversal synthetic cases
- add `SimpleLinkList<int|class|struct>` add synthetic cases
- add dictionary lookup variants for built-in comparer shape and custom `PreUtil` dictionary implementations
- wire the new cases into the existing synthetic catalog and current export flow
- update public benchmark documentation for the added custom-container families

## Out of Scope
- migrating `ITester`, `TestBooter`, or other `EffectiveTester` framework code
- changing the benchmark runner, exporter schema, or result aggregation format
- adding ratio reports, automated cross-run comparison, or analysis scripts
- broad package cleanup or container implementation changes outside minimal benchmark access needs

## Affected Area
- `Assets/UnityPerfLab/Runtime/Cases/Synthetic`
- `Assets/UnityPerfLab/README.md`
- `README.md`
- `docs/specs/20260323-004-custom-container-benchmark-expansion.md`

## Task Checklist
- [x] Add synthetic cases for `SimpleLinkList` traversal and add workloads across the current payload tiers.
- [x] Add synthetic cases for dictionary lookup variants that cover built-in and custom `PreUtil` dictionary shapes with explicit case labeling.
- [x] Register the new cases for `1K`, `100K`, and `1M` workloads and keep asmdef references/buildability correct.
- [x] Update user-facing README documentation for the expanded collection coverage and interpretation boundaries.
- [x] Validate Editor and Win64 IL2CPP Release Player runs export the new custom-container cases.

## Done When
The synthetic suite exports clearly named custom linked-list traversal/add cases and expanded dictionary lookup variants through the existing workflow, and the public docs explain what was added without changing the core run path or result schema.

## Validation

### Black-box Checks
- Run `UnityPerfLab/Run Synthetic Suite` and confirm exported results include the new custom-container case names.
- Build and run the Win64 IL2CPP Release Player and confirm its exported results include the same new case families.
- Check `summary.csv`, `overview.csv`, and `overview.md` for the `Collections` category and the expected built-in/custom dictionary and linked-list variants.

### White-box Needed
No

### White-box Trigger
This slice adds isolated synthetic benchmark cases and assembly references rather than modifying fragile branch-heavy benchmark core logic. Acceptance is primarily whether the suite builds, runs, and exports the expected case families.

### Internal Logic To Protect
N/A

## Write-back Needed
No

Stable behavior changes should stay in the task spec and README updates unless the new benchmark family later produces governance-relevant findings that belong in facts or a focused guide.

## Risks / Notes
- `EffectiveTester` naming and loop structure are reference input only; the migrated cases should match current container semantics even when the legacy loop shape was lossy.
- Dictionary cases that use different key types are useful for lookup-shape observation, but they should not be documented as direct apples-to-apples implementation winners.
- Black-box validation passed on 2026-03-23 with Unity `2022.3.16f1` using a clean execution copy under `H:\temp\unity-perf-lab-execute` because the primary workspace retained Unity project lock files after the first batch compile/import pass.
- Validated run outputs:
  - Editor: `H:\temp\unity-perf-lab-execute\PerfLabResults\20260323T122602418Z-dbf43d`
  - Win64 IL2CPP Release Player: `H:\temp\unity-perf-lab-execute\PlayerResults\20260323T122906222Z-e25155`
