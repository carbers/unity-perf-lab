# Specs Index

This directory contains the current UnityPerfLab MVP task spec set.

## Primary Spec

- `unity-perf-lab-mvp-task-spec.md`
  Parent MVP contract for the first UnityPerfLab delivery slice.

## Execution Order

1. `unity-perf-lab-mvp-01-bootstrap-task-spec.md`
   Bootstrap the Unity project root, module layout, asmdef boundaries, and bootstrap scene.
2. `unity-perf-lab-mvp-02-runner-reporting-task-spec.md`
   Build the runtime runner, measurement, environment capture, and CSV-first reporting core.
3. `unity-perf-lab-mvp-03-synthetic-suite-task-spec.md`
   Add the initial synthetic benchmark suite and Editor run entry points.
4. `unity-perf-lab-mvp-04-player-build-task-spec.md`
   Add Player autorun, Win64 IL2CPP Release build entry, and module-level documentation.

## Notes

- The parent spec defines the overall MVP contract.
- The four child specs narrow the work into reviewable slices.
- When implementation changes the stable role or boundaries of the repository, update `docs/facts/project-scope.md` instead of appending temporary task notes here.
