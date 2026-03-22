# UnityPerfLab

`UnityPerfLab` is a small benchmark infrastructure module for Unity and IL2CPP micro-optimization research. It is meant to keep synthetic performance experiments structured, rerunnable, and comparable over time instead of spreading ad-hoc benchmark scripts across a project.

## Structure

- `Runtime/Core`
  Runner, benchmark abstractions, measurement configs, stats, and shared constants.
- `Runtime/Cases/Synthetic`
  Pure benchmark cases such as traversal, dispatch, and closure overhead comparisons.
- `Runtime/Cases/RealWorld`
  Placeholder catalog for future cases that wrap real project code through adapters.
- `Runtime/Adapters`
  Adapter contracts for future real implementation reuse.
- `Runtime/Reporting`
  CSV and metadata export.
- `Runtime/Environment`
  Output path resolution and environment capture.
- `Runtime/Bootstrap`
  Suite composition and the Player autorun bootstrap behaviour.
- `Editor/Runner`
  Editor menu entry points.
- `Editor/Build`
  Scripted Win64 IL2CPP Release build entry.
- `Scenes`
  Minimal bootstrap scene for Player execution.

## asmdef Boundaries

- `UnityPerfLab.Runtime`
  Core runtime abstractions and reporting.
- `UnityPerfLab.Cases.Synthetic`
  Synthetic benchmark cases only.
- `UnityPerfLab.Cases.RealWorld`
  Future real-world cases.
- `UnityPerfLab.Bootstrap`
  Runtime composition root that assembles suites and runs the bootstrap scene.
- `UnityPerfLab.Editor`
  Editor menus and scripted build entry points.

Existing business/runtime code should depend on none of these assemblies. Future real-world cases should depend one-way on existing runtime assemblies, never the other way around.

## Running From The Editor

Use the Unity menu:

- `UnityPerfLab/Run Synthetic Suite`
- `UnityPerfLab/Run All Available Cases`

The current Synthetic suite includes:

- `Array_Int_ForEach_*` and `Array_Int_For_*`
- `List_Int_ForEach_*` and `List_Int_For_*`
- `List_Struct_ForEach_*` and `List_Struct_For_*`
- `DirectCall_*` and `InterfaceCall_*`
- `Closure_NoCapture_*` and `Closure_Capture_*`

Each family currently covers explicit size tiers:

- `1K`
- `100K`
- `1M`

Results are written under:

- `<ProjectRoot>/PerfLabResults/<run_id>/summary.csv`
- `<ProjectRoot>/PerfLabResults/<run_id>/raw_samples.csv`
- `<ProjectRoot>/PerfLabResults/<run_id>/metadata.json`

## Building Win64 IL2CPP Release

Use the Unity menu:

- `UnityPerfLab/Build/Win64 IL2CPP Release`

Or batchmode:

```powershell
Unity.exe -batchmode -projectPath <repo-root> -executeMethod UnityPerfLab.Editor.Build.PerfLabBuildMenu.BuildWin64Il2CppRelease -quit
```

The build output path is:

- `<ProjectRoot>/Builds/UnityPerfLab/Win64-IL2CPP-Release/UnityPerfLab.exe`

When the Player starts, the bootstrap scene runs the configured suite automatically and writes results to `Application.persistentDataPath/PerfLabResults/<run_id>/`.

Optional command-line overrides:

- `--suite=synthetic`
- `--suite=all`
- `--outputDir=<absolute-path>`

The default bootstrap scene path is:

- `Assets/UnityPerfLab/Scenes/UnityPerfLabBootstrap.unity`

## Adding A New Synthetic Case

1. Add a new `IPerfCase` implementation under `Runtime/Cases/Synthetic`.
2. Give it a clear descriptor name that encodes workload semantics and size.
3. Keep setup and teardown work outside the measured `Run(iterations)` path.
4. Write the result into `PerfVisibleSink` so the workload stays observable.
5. Register the case in `SyntheticPerfCaseCatalog.CreateAll()`.
6. Pick a workload-tier-appropriate measurement config instead of reusing one blindly.

## Adding A RealWorld Adapter And Case

1. Keep the real implementation in its existing runtime assembly.
2. Add a small adapter contract or wrapper under `Runtime/Adapters` when needed.
3. Implement the benchmark case under `Runtime/Cases/RealWorld`.
4. Register it in `RealWorldPerfCaseCatalog.CreateAll()`.

Do not copy real business code into `UnityPerfLab`, and do not make business assemblies depend back on `UnityPerfLab`.

## Known Limitations

- The current MVP is synthetic-case-first; `RealWorld` integration is only scaffolded.
- CSV is the primary export format; there is no ratio reporting or charting yet.
- Command-line support is intentionally minimal.
- The benchmark suite uses explicit registration instead of reflection discovery.
- The repo still relies on opening and saving the project in Unity once to let Unity regenerate any missing `.meta` files for newly added assets.
