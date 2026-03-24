# Simple Vs Standard Collections

## Scope

This fact records the current measured comparison between the `PreUtil` simple collections and the standard .NET collection baselines in `UnityPerfLab`.

Validated result sources:

- Editor run for dictionary and linked-list comparisons:
  - `PerfLabResults/20260323T080527707Z-acf8fd/`
  - primary evidence:
    - `overview.csv`
    - `overview.md`
- Win64 IL2CPP Release Player run for dictionary and linked-list comparisons:
  - `H:\temp\unity-perf-lab-execute\PlayerResults\20260323T122906222Z-e25155/`
  - primary evidence:
    - `summary.csv`
    - `overview.csv`
    - `overview.md`
- Editor run for `List<T>` vs `SimpleList<T>`:
  - `H:\temp\unity-perf-lab-execute-run2\PerfLabResults\20260324T035430968Z-89543a/`
  - primary evidence:
    - `summary.csv`
    - `overview.csv`
    - `summary.md`
- Win64 IL2CPP Release Player run for `List<T>` vs `SimpleList<T>`:
  - `H:\temp\unity-perf-lab-execute-run2\PlayerResults\20260324T035808896Z-5138bf/`
  - primary evidence:
    - `summary.csv`
    - `overview.csv`
    - `summary.md`

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

The current validated runs show six stable findings:

1. `SimpleIntDictionary<int>` is materially faster than `Dictionary<int,int>` for the measured sequential lookup shape, especially in the IL2CPP Release Player run.
2. `SimpleLinkList<T>` traversal is consistently faster than `LinkedList<T>` traversal across `int`, `class`, and `struct` payloads.
3. `SimpleLinkList<T>` add is dramatically faster than `LinkedList<T>` add for `int` and `struct` payloads, and still clearly faster for `class` payloads.
4. `SimpleList<T>` traversal is consistently faster than `List<T>` index traversal in the measured hot-path shape when `SimpleList<T>` is accessed through `buffer` plus `size`.
5. `SimpleList<T>` add is not a blanket replacement for `List<T>` add: `int` shows a modest Player win, `class` is mixed but trends slightly favorable at larger sizes, and `struct` is neutral to slower in the current Player run.
6. Non-`int` simple dictionary variants are useful lookup-shape observations, but only `SimpleIntDictionary<int>` is a strict apples-to-apples baseline comparison against `Dictionary<int,int>`.

Two observations should not be turned into blanket rules:

- `Dictionary<int,int>` with an explicit `EqualityComparer<int>.Default` does not show a stable, material difference from the plain built-in dictionary shape.
- `SimpleStringDictionary<int>` is not directly comparable to `Dictionary<int,int>` because the key type and hashing path are different.
- `SimpleList<T>` traversal results depend on its intended direct-access path; this report compares `List<T>` indexed reads against `SimpleList<T>.buffer` plus `SimpleList<T>.size`, not the obsolete indexer.

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

### `List<T>` traversal vs `SimpleList<T>` traversal

| Payload | Workload | `List<T>` median ns | `SimpleList<T>` median ns | `Simple` speedup |
| --- | --- | ---: | ---: | ---: |
| `int` | `1K` | `780.3253` | `368.3493` | `2.12x` |
| `int` | `100K` | `79648.0469` | `37618.3594` | `2.12x` |
| `int` | `1M` | `798543.7500` | `376909.3750` | `2.12x` |
| `class` | `1K` | `976.1163` | `630.1478` | `1.55x` |
| `class` | `100K` | `102771.0938` | `87514.0625` | `1.17x` |
| `class` | `1M` | `2694446.8750` | `2603509.3750` | `1.03x` |
| `struct` | `1K` | `1224.7075` | `583.8043` | `2.10x` |
| `struct` | `100K` | `120975.7813` | `58773.4375` | `2.06x` |
| `struct` | `1M` | `1215487.5000` | `570803.1250` | `2.13x` |

Interpretation:

- `SimpleList<T>` traversal wins for every measured payload kind and workload size in the current Player run.
- The strongest and most stable traversal benefit is on `int` and `struct`, where the win stays near `2.1x` across all three workload tiers.
- `class` traversal still favors `SimpleList<T>`, but the margin narrows sharply at `1M`, so this should be treated as a smaller optimization headroom than the value-type cases.

### `List<T>` add vs `SimpleList<T>` add

| Payload | Workload | `List<T>` median ns | `SimpleList<T>` median ns | `Simple` speedup |
| --- | --- | ---: | ---: | ---: |
| `int` | `1K` | `1356.2500` | `1212.3047` | `1.12x` |
| `int` | `100K` | `136850.0000` | `120350.0000` | `1.14x` |
| `int` | `1M` | `1379400.0000` | `1253400.0000` | `1.10x` |
| `class` | `1K` | `38084.7656` | `41866.2109` | `0.91x` |
| `class` | `100K` | `3924400.0000` | `3609800.0000` | `1.09x` |
| `class` | `1M` | `43213350.0000` | `38670600.0000` | `1.12x` |
| `struct` | `1K` | `4358.2031` | `4429.4922` | `0.98x` |
| `struct` | `100K` | `445250.0000` | `487800.0000` | `0.91x` |
| `struct` | `1M` | `4584400.0000` | `4984550.0000` | `0.92x` |

Interpretation:

- `SimpleList<int>` add is a real but modest win, staying around `1.10x` to `1.14x` in the current Player run.
- `class` add is mixed: `SimpleList<class>` loses at `1K`, then turns into a small win at `100K` and `1M`.
- `struct` add is effectively parity-to-slower for `SimpleList<T>` in this measured preallocated fill-from-empty shape, so the current data does not justify replacing `List<struct>` for add throughput alone.

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
- `SimpleList<T>` traversal still wins for every payload kind:
  - `int`: `2.43x` to `2.59x`
  - `class`: `1.17x` to `1.70x`
  - `struct`: `1.75x` to `2.02x`
- `SimpleList<T>` add remains mixed in Editor too:
  - `int`: `1.75x` to `1.89x` in favor of `SimpleList<T>`
  - `class`: about parity, with `List<T>` slightly ahead
  - `struct`: `List<T>` is faster by roughly `1.8x`

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

## Example Shapes

Use these examples as shape references for what the measured code paths actually look like.

### `List<T>` indexed traversal vs `SimpleList<T>` direct traversal

```csharp
var list = new List<int>(capacity);
for (int i = 0; i < capacity; i++)
{
    list.Add(i);
}

long listTotal = 0;
for (int i = 0; i < list.Count; i++)
{
    listTotal += list[i];
}

var simpleList = new SimpleList<int>(capacity);
for (int i = 0; i < capacity; i++)
{
    simpleList.Add(i);
}

long simpleListTotal = 0;
for (int i = 0; i < simpleList.size; i++)
{
    simpleListTotal += simpleList.buffer[i];
}
```

### Preallocated fill-from-empty add shape

```csharp
list.Clear();
for (int i = 0; i < count; i++)
{
    list.Add(i);
}

simpleList.Clear();
for (int i = 0; i < count; i++)
{
    simpleList.Add(i);
}
```

### `LinkedList<T>` foreach vs `SimpleLinkList<T>` node traversal

```csharp
long linkedListTotal = 0;
foreach (int value in linkedList)
{
    linkedListTotal += value;
}

long simpleLinkListTotal = 0;
for (int node = simpleLinkList.First; node != CollectionConst.INVALID_HEAD; node = simpleLinkList.Next(node))
{
    simpleLinkListTotal += simpleLinkList.GetValue(node);
}
```

## Practical Guidance

Use the current report this way:

- Prefer `SimpleIntDictionary<int>` over `Dictionary<int,int>` when the real workload matches this benchmark shape closely enough that lookup throughput is dominant.
- Treat `SimpleLinkList<T>` as the current measured winner over `LinkedList<T>` for both traversal and add-heavy workloads in this project.
- Treat `SimpleList<T>` traversal as a measured hot-path win only when the code can legitimately use its direct-access shape (`buffer` plus `size`) rather than the obsolete indexer.
- Do not assume `SimpleList<T>` add is a general replacement for `List<T>` add; the current Player data supports a small `int` win, mixed `class` behavior, and neutral-to-worse `struct` behavior.
- Keep `class` payload add results in context because object allocation is intentionally included in the measured path.
- Do not claim that `SimpleStringDictionary<int>` is globally better or worse than `Dictionary<int,int>`; it is measuring a different key regime.
- If a production decision depends on mutation patterns, removals, sparse keys, iteration invalidation rules, or memory behavior, add a dedicated benchmark before turning this fact into a coding standard.

## Open Follow-ups

The current report leaves these worthwhile follow-ups:

- compare `Dictionary<uint,int>` and `Dictionary<ulong,int>` against the matching simple dictionary key types for a fairer typed-key baseline
- add remove-heavy and mixed read/write workloads for the simple dictionaries
- add pooled-`SimpleList<T>` and non-preallocated `List<T>` / `SimpleList<T>` fills if construction policy becomes part of the real decision
- add memory / allocation profiling if container choice starts affecting runtime footprint rather than just throughput
