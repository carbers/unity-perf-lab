# Collections Benchmark Expansion

## Metadata

### Source Plan / Request
User-requested implementation of the reviewed `UnityPerfLab` collections benchmark expansion plan on 2026-03-23.

### Status
done

### Related Specs
- `docs/specs/2026-03-22-001-benchmark-mvp.md`

## Goal
Expand the synthetic benchmark suite with independent collection-focused cases migrated from `H:\DataStore\EffectiveTester` patterns without importing the old framework or changing the public run flow.

## In Scope
- add `Dictionary<int,int>` lookup synthetic cases
- add `LinkedList<int|class|struct>` foreach synthetic cases
- add `LinkedList<int|class|struct>` add synthetic cases
- add a narrow construction-oriented measurement default for add-heavy cases
- update public module docs for the expanded synthetic suite

## Out of Scope
- old `ITester` / `TestBooter` framework migration
- custom container migration such as `SimpleDictionary` or `SimpleLinkList`
- object pool, GC, Burst, geometry, or analysis-script migration
- new suites, result schemas, ratio reporting, or cross-run comparison

## Affected Area
- `Assets/UnityPerfLab/Runtime/Cases/Synthetic`
- `README.md`
- `Assets/UnityPerfLab/README.md`
- `docs/specs/20260323-001-collections-benchmark-expansion.md`

## Task Checklist
- [x] Add shared synthetic payload types and the three new collection case families.
- [x] Register all new cases for `1K`, `100K`, and `1M` workloads in the synthetic catalog.
- [x] Add construction-focused measurement defaults for linked-list add benchmarks.
- [x] Update user-facing README documentation for the expanded synthetic suite.
- [x] Validate Editor and Player synthetic runs export the new collection cases.

## Done When
The synthetic suite includes the planned 21 collection cases, they export through the existing result files without schema changes, and the public README documents the added benchmark families.

## Validation

### Black-box Checks
- Run `UnityPerfLab/Run Synthetic Suite` and confirm exported results include all new collection cases.
- Build and run the Win64 IL2CPP Release Player and confirm the exported Player results include the same new cases.
- Check `summary.csv`, `overview.csv`, and `overview.md` for the `Collections` category and the expected `lookup`, `foreach`, and `add` variants.

### White-box Needed
No

### White-box Trigger
This task adds new synthetic cases rather than changing fragile branch-heavy existing logic. Reviewable acceptance is based on successful execution and correct exported outputs.

### Internal Logic To Protect
N/A

## Write-back Needed
No

Stable behavior changes are documented in the task spec and README updates only.

## Risks / Notes
- `LinkedList` add cases intentionally include payload construction cost for `class` and `struct` payload variants.
- `1M` linked-list add cases can be expensive; construction-specific sample counts are reduced to keep smoke validation practical.
- Unity Editor and Win64 IL2CPP Release Player black-box validation both passed on 2026-03-23 using Unity `2022.3.16f1`.
