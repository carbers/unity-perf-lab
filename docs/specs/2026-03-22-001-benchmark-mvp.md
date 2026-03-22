# UnityPerfLab Phase 1 SPEC: Benchmark MVP

## 1. Goal

Create the first runnable closed loop for `UnityPerfLab` inside the current Unity project so synthetic microbenchmarks can be run consistently from the Editor and from a Win64 IL2CPP Release Player build.

This phase establishes the minimum maintainable benchmark infrastructure for validating Unity / IL2CPP optimization ideas over time without turning the module into a one-off script pile.

## 2. Scope

Phase 1 includes:

- `Assets/UnityPerfLab/` as an isolated module inside the current Unity project
- asmdef isolation for runtime, Editor, and synthetic cases
- core benchmark abstractions and a reusable runner
- `Stopwatch`-based measurement with `FixedIterations` and simplified `TargetDuration`
- CSV-first result export:
  - `summary.csv`
  - `raw_samples.csv`
- a minimal bootstrap scene that autoruns the synthetic suite in Player
- an Editor menu entry to run the synthetic suite
- a scripted Win64 IL2CPP Release build entry
- the first set of synthetic benchmark cases:
  - `int[]` for vs foreach
  - `List<int>` for vs foreach
  - `List<struct>` for vs foreach
  - direct call vs interface call
  - closure capture vs no capture

## 3. Non-Goals / Out Of Scope

Phase 1 does not include:

- deep real-world runtime integrations
- baseline or ratio analytics
- cross-version comparison tooling
- automated multi-platform build matrices
- a large custom Editor window
- reflection-based case discovery
- a plugin architecture
- advanced metadata or comparison storage
- Phase 2+ benchmark families beyond the synthetic set listed in this document

Minimal future extension points may remain in the tree, but they are not part of the Phase 1 public surface.

## 4. Directory / File Changes

Phase 1 uses and tightens the existing module structure:

- `Assets/UnityPerfLab/Runtime/Core`
- `Assets/UnityPerfLab/Runtime/Cases/Synthetic`
- `Assets/UnityPerfLab/Runtime/Cases/RealWorld`
- `Assets/UnityPerfLab/Runtime/Adapters`
- `Assets/UnityPerfLab/Runtime/Reporting`
- `Assets/UnityPerfLab/Runtime/Environment`
- `Assets/UnityPerfLab/Runtime/Bootstrap`
- `Assets/UnityPerfLab/Editor/Build`
- `Assets/UnityPerfLab/Editor/Runner`
- `Assets/UnityPerfLab/Editor/Export`
- `Assets/UnityPerfLab/Scenes`
- `Assets/UnityPerfLab/Config`

Phase 1 must contain at least these assembly boundaries:

- `UnityPerfLab.Runtime`
- `UnityPerfLab.Cases.Synthetic`
- `UnityPerfLab.Editor`

Additional Phase 1 files introduced or updated by this corrective pass:

- `docs/specs/2026-03-22-001-benchmark-mvp.md`
- `docs/specs/README.md`
- `Assets/UnityPerfLab/README.md`
- Phase 1 runtime and Editor entry points needed to keep the public surface synthetic-only

## 5. Core Abstractions To Introduce

Phase 1 keeps the current benchmark core centered on these types:

- `IPerfCase`
- `PerfCaseDescriptor`
- `PerfRunRequest`
- `PerfRunResult`
- `PerfSample`
- `PerfRunner`

Supporting types kept in scope for Phase 1:

- `PerfMeasurementMode`
- `PerfMeasurementConfig`
- `PerfCaseRunResult`
- `PerfSummaryStats`
- `PerfVisibleSink`
- `IResultExporter`

The benchmark case contract is:

- setup and teardown are explicit
- only `Run(int iterations)` is measured
- each case publishes a descriptor with stable case metadata
- each case consumes its result through `PerfVisibleSink`

## 6. Runtime Flow

### Editor Synthetic Run

1. The user triggers `UnityPerfLab/Run Synthetic Suite`.
2. `PerfLabEditorRunner` requests the synthetic suite.
3. `PerfSuiteCatalog` returns the explicitly registered synthetic cases.
4. `PerfRunner` executes each case in this order:
   - `GlobalSetup`
   - resolve iterations per sample
   - warmup samples
   - measured samples
   - `GlobalTeardown`
5. `CsvResultExporter` writes `summary.csv` and `raw_samples.csv`, plus `metadata.json`.

### Player Run

1. The built Player starts in `Assets/UnityPerfLab/Scenes/UnityPerfLabBootstrap.unity`.
2. `PerfLabBootstrapBehaviour` resolves command-line overrides and defaults to the synthetic suite.
3. `PerfLabExecution` runs the synthetic suite through `PerfRunner`.
4. Results are exported to the resolved output directory.
5. The Player exits after completion when not running inside the Editor.

## 7. Build / Run Flow

### Editor Run

- Run from Unity via `UnityPerfLab/Run Synthetic Suite`.
- Results default to `<ProjectRoot>/PerfLabResults/<run_id>/`.

### Win64 IL2CPP Release Build

- Build from Unity via `UnityPerfLab/Build/Win64 IL2CPP Release`.
- Build target:
  - `StandaloneWindows64`
  - `IL2CPP`
  - `development = false`
- Output path:
  - `<ProjectRoot>/Builds/UnityPerfLab/Win64-IL2CPP-Release/UnityPerfLab.exe`

### Player Output

- Default output path:
  - `Application.persistentDataPath/PerfLabResults/<run_id>/`
- Minimal command-line override documented in Phase 1:
  - `--outputDir=<absolute-path>`

If `--suite` support remains in code, Phase 1 only accepts `synthetic`.

## 8. Benchmark Methodology Rules

The following rules apply to all Phase 1 benchmarks:

- `GlobalSetup`, `Setup`, `Teardown`, and `GlobalTeardown` are not included in measured time.
- Only `Run(iterations)` is measured.
- `Stopwatch` is used internally.
- Results are normalized to `ns/op`.
- Every benchmark writes its result to `PerfVisibleSink`.
- Two measurement modes are supported:
  - `FixedIterations`
  - simplified `TargetDuration`
- Simplified `TargetDuration` means:
  - run one probe
  - estimate iterations for the target duration
  - clamp between min and max
  - keep the resolved iteration count fixed for all warmup and measured samples

CSV is the primary output format.

`summary.csv` must include at least:

- `run_id`
- `timestamp`
- `unity_version`
- `platform`
- `build_config`
- `case_name`
- `category`
- `parameters`
- `measurement_mode`
- `warmup_count`
- `sample_count`
- `total_iterations`
- `target_duration_ms`
- `mean_ns`
- `median_ns`
- `min_ns`
- `max_ns`

`raw_samples.csv` must include at least:

- `run_id`
- `case_name`
- `sample_index`
- `is_warmup`
- `iterations`
- `elapsed_ns`
- `ns_per_op`

Suggested defaults for Phase 1:

- light cases:
  - warmup `5`
  - measure `20`
  - target sample duration `200 ms`
- heavier cases:
  - warmup `3`
  - measure `10`
  - target sample duration `500 ms`

## 9. Cases Included In This Phase

Phase 1 includes these synthetic case families:

- `int[]` for vs foreach
- `List<int>` for vs foreach
- `List<struct>` for vs foreach
- direct call vs interface call
- closure capture vs no capture

Each relevant family must support these workload sizes:

- `1K`
- `100K`
- `1M`

Case names must encode workload semantics, for example:

- `Array_Int_For_1K`
- `Array_Int_ForEach_100K`
- `List_Int_For_100K`
- `InterfaceCall_1M`

Phase 1 category values are intentionally small and stable:

- `Looping`
- `Dispatch`
- `Closure`

## 10. Acceptance Criteria

Phase 1 is complete when all of the following are true:

1. Synthetic cases can be run from the Unity Editor through `UnityPerfLab/Run Synthetic Suite`.
2. `summary.csv` and `raw_samples.csv` are generated for each run.
3. A Win64 IL2CPP Release Player can be built through `UnityPerfLab/Build/Win64 IL2CPP Release`.
4. The Player starts, runs the bootstrap flow, writes results, and exits.
5. The code structure is not a single giant script.
6. `Assets/UnityPerfLab/README.md` explains:
   - purpose
   - structure
   - how to run
   - how to build
   - how to add a synthetic case
7. The repository contains this real Phase 1 SPEC document.
8. Phase 1 public entry points are synthetic-only.

## 11. Risks / Assumptions

- The current `UnityPerfLab` implementation is treated as a reusable base, not a rewrite target.
- Unity `2022.3.20f1` is the expected local Editor version for validation.
- Editor benchmark numbers are useful for smoke validation, but IL2CPP Release Player runs remain the real validation target.
- Extremely fast workloads may still require future tuning of `TargetDuration` clamp values.
- Closure capture benchmarks intentionally include allocation and capture effects; results should be interpreted as pattern comparisons, not as generic function-call cost.
- Real-world adapters and case catalogs may remain as inert extension points, but they are not part of the Phase 1 public workflow.

## 12. Brief Note About Next Phases

- `002`: collections and data layout cases
- `003`: boxing, generics, and abstraction cases
- `004`: real-world adapters and real implementation integration
- `005`: configuration, versioning, and regression comparison capabilities
