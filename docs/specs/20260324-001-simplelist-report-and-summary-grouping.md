# SimpleList Report And Summary Grouping

## Metadata

### Source Plan / Request
User-requested follow-up on 2026-03-24 to add missing `SimpleList` vs `List` benchmark coverage, add reusable example-code guidance for facts, and make the Markdown run summary easier to read by grouping output by category.

### Status
done

### Related Specs
- `docs/specs/20260323-001-collections-benchmark-expansion.md`
- `docs/specs/20260323-004-custom-container-benchmark-expansion.md`
- `docs/specs/20260323-005-simple-vs-standard-collections-report.md`

## Goal
Add a narrow reviewable slice that closes the missing `SimpleList` vs `List` comparison gap, improves the reusable collections fact with example code, and updates Markdown summary output so larger benchmark suites remain readable.

## In Scope
- add synthetic benchmark cases for direct `List<T>` vs `SimpleList<T>` comparison
- keep the benchmark export flow stable while improving Markdown summary grouping by category
- emit a Markdown summary file that matches the categorized layout
- update stable write-back guidance so future benchmark-backed facts include example code when that usage shape is part of the reusable takeaway
- update the collections fact once validated comparison data exists

## Out of Scope
- redesigning CSV schemas
- adding cross-run diff tooling or ratio automation
- broad benchmark-suite taxonomy changes outside the Markdown summary layout
- changing `PreUtil` container implementations

## Affected Area
- `Assets/UnityPerfLab/Runtime/Cases/Synthetic`
- `Assets/UnityPerfLab/Runtime/Reporting`
- `Assets/UnityPerfLab/README.md`
- `README.md`
- `.cursor/rules/*` or equivalent repo-wide write-back guidance
- `docs/facts/simple-vs-standard-collections.md`

## Task Checklist
- [x] Add direct `List<T>` vs `SimpleList<T>` synthetic cases with explicit naming and parameters.
- [x] Update Markdown summary export to group rows by category and then size.
- [x] Keep `overview.md` readable and align the Markdown summary naming with the new grouped output.
- [x] Add stable rule guidance for example code in reusable benchmark-backed facts.
- [x] Validate the new cases and update the collections fact only with validated numbers.

## Done When
The synthetic suite contains direct `SimpleList` vs `List` cases, Markdown summary output is grouped by category for easier reading, and the collections fact documents the new comparison with example code backed by a validated run.

## Validation

### Black-box Checks
- Run the synthetic suite and confirm exported results include the new `List` / `SimpleList` comparison cases.
- Check the run directory contains categorized Markdown summary output and that rows are grouped by category before size sections.
- Reconcile any new numbers written into `docs/facts/simple-vs-standard-collections.md` against the validated result files.

### White-box Needed
No

### White-box Trigger
This slice mostly adds isolated benchmark cases and presentation formatting. Acceptance is based on successful export shape and validated benchmark output, not fragile internal branch protection.

### Internal Logic To Protect
N/A

## Write-back Needed
Yes

Write back only the stable collections fact update and the reusable write-back guidance for example code.

## Risks / Notes
- `SimpleList<T>` has a non-standard access pattern because direct `buffer` and `size` access is part of its intended fast path, so any comparison must make that usage shape explicit.
- Validation completed on 2026-03-24 with:
  - Editor: `H:\temp\unity-perf-lab-execute-run2\PerfLabResults\20260324T035430968Z-89543a`
  - Win64 IL2CPP Release Player: `H:\temp\unity-perf-lab-execute-run2\PlayerResults\20260324T035808896Z-5138bf`
