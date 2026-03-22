# Project Scope

## Current Phase Goal
Establish `unity-perf-lab` as a Unity project with an isolated `UnityPerfLab` benchmark module for repeatable performance experiments.

## In Scope
- repository-level SOP guidance
- a Unity project rooted at the repository root
- the `Assets/UnityPerfLab` benchmark module
- synthetic benchmark infrastructure and initial synthetic cases
- Editor benchmark entry points
- Win64 IL2CPP Release build entry
- structured benchmark result export

## Out of Scope
- product gameplay or business features
- a large plugin architecture
- reflection-based benchmark discovery
- charting and cross-version automation matrices
- speculative abstractions for future platforms

## Key Constraints
- `UnityPerfLab` must remain logically isolated and asmdef-isolated
- business/runtime code may depend on nothing in `UnityPerfLab`
- the MVP must prioritize a working build/run/export loop
- Release + IL2CPP is the primary validation path

## Current Assumptions
- Unity `2022.3 LTS` is the baseline target
- CSV is the main result format
- real-world benchmark cases will be added later when reusable runtime code exists
- the repository continues to keep SOP docs alongside the Unity project

## Open Risks
- local Unity patch version may differ from the pinned `ProjectVersion.txt`
- validation is limited until Unity is available on the machine running the repo
- very fast synthetic cases may still need manual tuning of target-duration clamps over time
