# UnityPerfLab

`UnityPerfLab` is a small benchmark and performance experiment module for Unity and IL2CPP optimization research.

Phase 1 focuses on a synthetic benchmark MVP that is easy to rerun, export, and extend without turning the project into a pile of ad-hoc scripts.

## Purpose

- keep synthetic benchmark cases isolated from normal runtime code
- validate optimization hypotheses with a repeatable runner
- export stable CSV outputs for later comparison
- run the same synthetic suite from the Editor and from a Win64 IL2CPP Release Player

## Structure

- `Runtime/Core`
  Runner, case abstractions, measurement config, stats, and visible sink.
- `Runtime/Cases/Synthetic`
  Phase 1 synthetic benchmark cases.
- `Runtime/Cases/RealWorld`
  Placeholder for future phases only. Not part of the Phase 1 public workflow.
- `Runtime/Adapters`
  Minimal future extension point for real-world integrations.
- `Runtime/Reporting`
  CSV and metadata export.
- `Runtime/Environment`
  Environment capture and output path resolution.
- `Runtime/Bootstrap`
  Runtime composition and Player autorun bootstrap.
- `Editor/Runner`
  Editor run entry point.
- `Editor/Build`
  Win64 IL2CPP Release build entry point.
- `Scenes`
  Minimal bootstrap scene for Player execution.

## Phase 1 Boundaries

- Public benchmark workflow is synthetic-only.
- Case registration is explicit, not reflection-based.
- CSV is the primary output format.
- Real-world integrations, comparison tooling, and automation come in later phases.

## Running The Synthetic Suite

Use the Unity menu:

- `UnityPerfLab/Run Synthetic Suite`

The current synthetic suite includes:

- `Array_Int_For_*` and `Array_Int_ForEach_*`
- `List_Int_For_*` and `List_Int_ForEach_*`
- `List_Struct_For_*` and `List_Struct_ForEach_*`
- `Dictionary_Int_Lookup_*`
- `LinkedList_Int_ForEach_*`, `LinkedList_Class_ForEach_*`, and `LinkedList_Struct_ForEach_*`
- `LinkedList_Int_Add_*`, `LinkedList_Class_Add_*`, and `LinkedList_Struct_Add_*`
- `DirectCall_*` and `InterfaceCall_*`
- `Closure_NoCapture_*` and `Closure_Capture_*`
- `StaticProperty_VectorLike_*`
- `ConstScalar_*` and `StaticReadonlyScalar_*`
- `StaticInit_DeclarationOnly_*` and `StaticInit_ExplicitCctor_*`
- `StaticHolder_FieldAccess_*` and `StaticHolder_PropertyAccess_*`
- `HelperCall_Flat_*` and `HelperCall_Chain_*`
- `HelperParams_FixedArity_*` and `HelperParams_Params_*`

Each family covers these workload tiers:

- `1K`
- `100K`
- `1M`

See [`docs/guides/il2cpp-semantic-overhead-benchmarks.md`](../../docs/guides/il2cpp-semantic-overhead-benchmarks.md) for the hypothesis mapping and interpretation limits of the semantic-overhead cases.

## Output Locations

Editor runs write to:

- `<ProjectRoot>/PerfLabResults/<run_id>/summary.csv`
- `<ProjectRoot>/PerfLabResults/<run_id>/raw_samples.csv`
- `<ProjectRoot>/PerfLabResults/<run_id>/overview.csv`
- `<ProjectRoot>/PerfLabResults/<run_id>/overview.md`
- `<ProjectRoot>/PerfLabResults/<run_id>/metadata.json`

Player runs write to:

- `Application.persistentDataPath/PerfLabResults/<run_id>/`
- The same directory also includes `overview.csv` and `overview.md` for quick reading.

Optional Player override:

- `--outputDir=<absolute-path>`

## Building Win64 IL2CPP Release

Use the Unity menu:

- `UnityPerfLab/Build/Win64 IL2CPP Release`

Or batchmode:

```powershell
Unity.exe -batchmode -projectPath <repo-root> -executeMethod UnityPerfLab.Editor.Build.PerfLabBuildMenu.BuildWin64Il2CppRelease -quit
```

The build output path is:

- `<ProjectRoot>/Builds/UnityPerfLab/Win64-IL2CPP-Release/UnityPerfLab.exe`

The Player starts in:

- `Assets/UnityPerfLab/Scenes/UnityPerfLabBootstrap.unity`

On startup, the bootstrap flow runs the synthetic suite automatically, exports results, and exits.

## Adding A Synthetic Case

1. Add a new `IPerfCase` implementation under `Runtime/Cases/Synthetic`.
2. Give it a descriptor name that encodes the workload semantics and size.
3. Keep setup and teardown work outside the measured `Run(iterations)` path.
4. Write the final result into `PerfVisibleSink`.
5. Register the case explicitly in `SyntheticPerfCaseCatalog.CreateAll()`.
6. Choose a measurement config that matches the workload tier.

## Roadmap

- `002`: additional collections and data layout cases
- `003`: boxing, generics, and abstraction cases
- `004`: real-world adapters and implementation integration
- `005`: configuration, versioning, and regression comparison

## Known Limitations

- Phase 1 is synthetic-first and synthetic-only at the public workflow level.
- There is no ratio reporting, charting, or regression comparison yet.
- Command-line support is intentionally minimal.
- IL2CPP Release Player runs remain the real validation target for optimization conclusions.
