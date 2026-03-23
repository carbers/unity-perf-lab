# Simple Vs Standard Collections

## Scope

This fact records the current measured comparison between the `PreUtil` simple collections and the standard .NET collection baselines in `UnityPerfLab`.

Validated result sources:

- Editor run:
  - `PerfLabResults/20260323T080527707Z-acf8fd/`
  - primary evidence:
    - `overview.csv`
    - `overview.md`
- Win64 IL2CPP Release Player run:
  - `H:\temp\unity-perf-lab-execute\PlayerResults\20260323T122906222Z-e25155/`
  - primary evidence:
    - `summary.csv`
    - `overview.csv`
    - `overview.md`

Runtime identity:

- Unity `2022.3.16f1`
- Editor comparison environment:
  - `platform = WindowsEditor`
  - `build_config = Editor`
- Player comparison environment:
  - `platform = WindowsPlayer`
  - `build_config = ReleasePlayer`
  - `scripting backend = IL2CPP`

Use this fact as a benchmark-backed comparison reference for future container choices and follow-up experiments, not as a claim that every production workload will behave the same way.

## Executive Summary

The current validated runs show four stable findings:

1. `SimpleIntDictionary<int>` is materially faster than `Dictionary<int,int>` for the measured sequential lookup shape, especially in the IL2CPP Release Player run.
2. `SimpleLinkList<T>` traversal is consistently faster than `LinkedList<T>` traversal across `int`, `class`, and `struct` payloads.
3. `SimpleLinkList<T>` add is dramatically faster than `LinkedList<T>` add for `int` and `struct` payloads, and still clearly faster for `class` payloads.
4. Non-`int` simple dictionary variants are useful lookup-shape observations, but only `SimpleIntDictionary<int>` is a strict apples-to-apples baseline comparison against `Dictionary<int,int>`.

Two observations should not be turned into blanket rules:

- `Dictionary<int,int>` with an explicit `EqualityComparer<int>.Default` does not show a stable, material difference from the plain built-in dictionary shape.
- `SimpleStringDictionary<int>` is not directly comparable to `Dictionary<int,int>` because the key type and hashing path are different.

## Direct Player Comparisons

The IL2CPP Release Player run is the primary basis for conclusions.

### `Dictionary<int,int>` lookup vs `SimpleIntDictionary<int>` lookup

| Workload | `Dictionary<int,int>` median ns | `SimpleIntDictionary<int>` median ns | `Simple` speedup |
| --- | ---: | ---: | ---: |
| `1K` | `9484.2613` | `2003.8916` | `4.73x` |
| `100K` | `957510.5469` | `200491.4063` | `4.78x` |
| `1M` | `9564443.7500` | `2031546.8750` | `4.71x` |

Interpretation:

- The speedup is large and very stable across all three workload tiers.
- The current benchmark shape is sequential key lookup over a densely populated container with setup outside the measured path.
- In this measured shape, `SimpleIntDictionary<int>` is the clearest win among the directly comparable dictionary baselines.

### `LinkedList<T>` traversal vs `SimpleLinkList<T>` traversal

| Payload | Workload | `LinkedList<T>` median ns | `SimpleLinkList<T>` median ns | `Simple` speedup |
| --- | --- | ---: | ---: | ---: |
| `int` | `1K` | `6021.0074` | `2013.1343` | `2.99x` |
| `int` | `100K` | `606861.3281` | `200077.7344` | `3.03x` |
| `int` | `1M` | `7579215.6250` | `1993475.0000` | `3.80x` |
| `class` | `1K` | `11937.0049` | `2191.3468` | `5.45x` |
| `class` | `100K` | `1226652.3438` | `220201.5625` | `5.57x` |
| `class` | `1M` | `13980931.2500` | `3974493.7500` | `3.52x` |
| `struct` | `1K` | `5982.8412` | `2526.7370` | `2.37x` |
| `struct` | `100K` | `605267.1875` | `260680.0781` | `2.32x` |
| `struct` | `1M` | `7817806.2500` | `2568550.0000` | `3.04x` |

Interpretation:

- `SimpleLinkList<T>` traversal wins for every measured payload kind and workload size.
- The largest traversal win is for `class` payloads.
- The relative win stays meaningful even at `1M`, which suggests the result is not just small-size noise.

### `LinkedList<T>` add vs `SimpleLinkList<T>` add

| Payload | Workload | `LinkedList<T>` median ns | `SimpleLinkList<T>` median ns | `Simple` speedup |
| --- | --- | ---: | ---: | ---: |
| `int` | `1K` | `93092.7734` | `7882.6172` | `11.81x` |
| `int` | `100K` | `9123050.0000` | `817750.0000` | `11.16x` |
| `int` | `1M` | `91695200.0000` | `8218050.0000` | `11.16x` |
| `class` | `1K` | `143729.4922` | `54764.2578` | `2.62x` |
| `class` | `100K` | `13918550.0000` | `5222800.0000` | `2.66x` |
| `class` | `1M` | `136140550.0000` | `56663950.0000` | `2.40x` |
| `struct` | `1K` | `95417.5781` | `9576.7578` | `9.96x` |
| `struct` | `100K` | `9908400.0000` | `1026200.0000` | `9.66x` |
| `struct` | `1M` | `95587450.0000` | `10470800.0000` | `9.13x` |

Interpretation:

- `SimpleLinkList<T>` add is the strongest measured collection win in this report.
- `int` and `struct` payloads show roughly one order of magnitude improvement.
- `class` payloads still favor `SimpleLinkList<T>`, but the relative win is smaller because payload allocation remains part of the measured path.

## Editor Confirmation

The Editor run is not the acceptance target, but it broadly confirms the same ordering:

- `SimpleIntDictionary<int>` still beats `Dictionary<int,int>`, but only by about `1.30x` to `1.33x`.
- `SimpleLinkList<T>` traversal still wins for every payload kind:
  - `int`: `1.41x` to `1.60x`
  - `class`: `3.95x` to `4.68x`
  - `struct`: `2.85x` to `3.27x`
- `SimpleLinkList<T>` add still wins for every payload kind:
  - `int`: about `9.1x`
  - `class`: about `2.0x`
  - `struct`: about `7.1x` to `7.4x`

Interpretation:

- The qualitative ranking is consistent between Editor and Player.
- The Player deltas are often larger, especially for dictionary lookup, so future conclusions should continue to prefer IL2CPP Release Player evidence.

## Lookup-Shape Observations

The following Player lookup medians are useful for relative shape observation, but they are not strict baseline replacements for `Dictionary<int,int>`:

### Player lookup ranking

| Workload | Fastest to slowest |
| --- | --- |
| `1K` | `SimpleIntDictionary<int>` -> `SimpleUlongDictionary<int>` -> `SimpleUIntDictionary<int>` -> `SimpleStringDictionary<int>` -> `Dictionary<int,int>` -> `Dictionary<int,int>(EqualityComparer<int>.Default)` |
| `100K` | `SimpleIntDictionary<int>` -> `SimpleUIntDictionary<int>` -> `SimpleUlongDictionary<int>` -> `SimpleStringDictionary<int>` -> `Dictionary<int,int>(EqualityComparer<int>.Default)` -> `Dictionary<int,int>` |
| `1M` | `SimpleIntDictionary<int>` -> `SimpleUIntDictionary<int>` -> `SimpleUlongDictionary<int>` -> `Dictionary<int,int>` -> `Dictionary<int,int>(EqualityComparer<int>.Default)` -> `SimpleStringDictionary<int>` |

Interpretation:

- `SimpleUIntDictionary<int>` and `SimpleUlongDictionary<int>` stay close to `SimpleIntDictionary<int>`, but remain slightly slower in the current Player run.
- `SimpleStringDictionary<int>` scales much worse at `1M`, which is consistent with a more expensive key path rather than an implementation bug by itself.
- The explicit default comparer on the standard dictionary is effectively neutral in Player and mixed in Editor, so this report does not justify a rule either way.

## Practical Guidance

Use the current report this way:

- Prefer `SimpleIntDictionary<int>` over `Dictionary<int,int>` when the real workload matches this benchmark shape closely enough that lookup throughput is dominant.
- Treat `SimpleLinkList<T>` as the current measured winner over `LinkedList<T>` for both traversal and add-heavy workloads in this project.
- Keep `class` payload add results in context because object allocation is intentionally included in the measured path.
- Do not claim that `SimpleStringDictionary<int>` is globally better or worse than `Dictionary<int,int>`; it is measuring a different key regime.
- If a production decision depends on mutation patterns, removals, sparse keys, iteration invalidation rules, or memory behavior, add a dedicated benchmark before turning this fact into a coding standard.

## Open Follow-ups

The current report leaves these worthwhile follow-ups:

- compare `Dictionary<uint,int>` and `Dictionary<ulong,int>` against the matching simple dictionary key types for a fairer typed-key baseline
- add remove-heavy and mixed read/write workloads for the simple dictionaries
- add memory / allocation profiling if container choice starts affecting runtime footprint rather than just throughput
