# IL2CPP Semantic Overhead Benchmarks

This guide covers the synthetic benchmark family added to `UnityPerfLab` for hot-path semantic-overhead and micro-optimization experiments.

## Purpose

These cases provide a controlled baseline for source-level patterns that often come up in Unity and IL2CPP optimization work:

- static property access inside a hot loop vs hoisting to a local
- `const` scalar access vs `static readonly` scalar access
- declaration-only static initialization vs explicit static constructor initialization
- thin helper chains vs a flat helper body
- `params` convenience helpers vs fixed-arity helpers
- custom static-holder field access vs static property access

The family is synthetic-only. It is meant to show observable differences for tightly controlled code shapes, not to stand in for full gameplay or engine behavior.

## Case Families

The suite currently adds these benchmark pairs for each workload tier `1K`, `100K`, and `1M`:

- `StaticProperty_VectorLike_InLoop_*` vs `StaticProperty_VectorLike_HoistedLocal_*`
  Measures a vector-like static property access inside the loop body against reading the same value into a local before the loop.
- `ConstScalar_InLoop_*` vs `StaticReadonlyScalar_InLoop_*`
  Compares scalar constant access shape against `static readonly` scalar access in the same loop body.
- `StaticInit_DeclarationOnly_Field_*` vs `StaticInit_ExplicitCctor_Field_*`
  Compares repeated access through holder types that differ mainly by declaration-only initialization versus an explicit static constructor.
- `StaticHolder_FieldAccess_*` vs `StaticHolder_PropertyAccess_*`
  Compares static readonly field access with a trivial static property over the same holder state.
- `HelperCall_Flat_*` vs `HelperCall_Chain_*`
  Compares a flat helper body against the same math expressed through a thin helper chain.
- `HelperParams_FixedArity_*` vs `HelperParams_Params_*`
  Compares fixed-arity helper calls against `params` convenience calls inside the measured path.

## What These Results Can And Cannot Prove

These cases can:

- show that a controlled code-shape difference is observable under the current runtime and build mode
- help decide whether a follow-up IL2CPP player experiment is worth deeper investigation
- give a stable baseline for later source-level optimization discussions

These cases cannot:

- prove undocumented Unity engine internals
- prove why IL2CPP generated code behaves a certain way without inspecting generated output separately
- replace validation in a real gameplay or subsystem context

## Running The Cases

Use the existing synthetic workflow. No separate suite or special entry point was added.

Editor run:

- Unity menu: `UnityPerfLab/Run Synthetic Suite`

Win64 IL2CPP Release Player:

- Unity menu: `UnityPerfLab/Build/Win64 IL2CPP Release`
- Run the built player at `Builds/UnityPerfLab/Win64-IL2CPP-Release/UnityPerfLab.exe`

The new cases export through the same files as the rest of the synthetic suite:

- `summary.csv`
- `overview.csv`
- `overview.md`

Look for the `SemanticOverhead` category and the `subject` / `variant` parameter pairings when comparing results.

## Interpreting Editor Vs IL2CPP Player Results

Treat Editor results as smoke validation and rough directional context only.

Use Win64 IL2CPP Release Player results for actual optimization decisions because:

- Editor runs include Editor-specific overhead and a different execution environment
- IL2CPP code generation and static initialization behavior can differ materially from Editor behavior
- helper inlining, property access shape, and `params` overhead may look different once compiled into the Player

If a pattern appears only in the Editor and disappears in IL2CPP Release Player, prefer the Player result.
If a pattern appears in both, treat the IL2CPP result as the authoritative one for later follow-up.
