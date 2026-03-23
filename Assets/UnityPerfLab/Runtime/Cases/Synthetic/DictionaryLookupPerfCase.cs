using System.Globalization;
using System.Collections.Generic;
using Perf.Util;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class DictionaryLookupPerfCase : PerfCaseBase
    {
        public enum LookupVariant
        {
            DictionaryInt,
            DictionaryIntExplicitDefaultComparer,
            SimpleIntDictionary,
            SimpleUIntDictionary,
            SimpleUlongDictionary,
            SimpleStringDictionary
        }

        private readonly LookupVariant variant;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private Dictionary<int, int> dictionaryValues;
        private Dictionary<int, int> dictionaryValuesWithDefaultComparer;
        private SimpleIntDictionary<int> simpleIntValues;
        private SimpleUIntDictionary<int> simpleUIntValues;
        private SimpleUlongDictionary<int> simpleUlongValues;
        private SimpleStringDictionary<int> simpleStringValues;
        private string[] stringKeys;

        public DictionaryLookupPerfCase(int workloadSize)
            : this(LookupVariant.DictionaryInt, workloadSize)
        {
        }

        public DictionaryLookupPerfCase(LookupVariant variant, int workloadSize)
        {
            this.variant = variant;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                GetCaseName(variant, workloadSize),
                "Collections",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "collection", GetCollectionLabel(variant) },
                    { "loop", "lookup" },
                    { "size", workloadSize.ToString() },
                    { "variant", GetVariantLabel(variant) }
                });
        }

        public override PerfCaseDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public override void GlobalSetup()
        {
            switch (variant)
            {
                case LookupVariant.DictionaryInt:
                    dictionaryValues = new Dictionary<int, int>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        dictionaryValues.Add(i, i);
                    }

                    break;

                case LookupVariant.DictionaryIntExplicitDefaultComparer:
                    dictionaryValuesWithDefaultComparer = new Dictionary<int, int>(workloadSize, EqualityComparer<int>.Default);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        dictionaryValuesWithDefaultComparer.Add(i, i);
                    }

                    break;

                case LookupVariant.SimpleIntDictionary:
                    simpleIntValues = new SimpleIntDictionary<int>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        simpleIntValues.Add(i, i);
                    }

                    break;

                case LookupVariant.SimpleUIntDictionary:
                    simpleUIntValues = new SimpleUIntDictionary<int>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        simpleUIntValues.Add((uint)i, i);
                    }

                    break;

                case LookupVariant.SimpleUlongDictionary:
                    simpleUlongValues = new SimpleUlongDictionary<int>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        simpleUlongValues.Add((ulong)i, i);
                    }

                    break;

                default:
                    simpleStringValues = new SimpleStringDictionary<int>(workloadSize);
                    stringKeys = new string[workloadSize];
                    for (int i = 0; i < workloadSize; i++)
                    {
                        string key = i.ToString(CultureInfo.InvariantCulture);
                        stringKeys[i] = key;
                        simpleStringValues.Add(key, i);
                    }

                    break;
            }
        }

        public override void Run(int iterations)
        {
            long total = 0;
            switch (variant)
            {
                case LookupVariant.DictionaryInt:
                    for (int iteration = 0; iteration < iterations; iteration++)
                    {
                        for (int i = 0; i < workloadSize; i++)
                        {
                            total += dictionaryValues[i];
                        }
                    }

                    break;

                case LookupVariant.DictionaryIntExplicitDefaultComparer:
                    for (int iteration = 0; iteration < iterations; iteration++)
                    {
                        for (int i = 0; i < workloadSize; i++)
                        {
                            total += dictionaryValuesWithDefaultComparer[i];
                        }
                    }

                    break;

                case LookupVariant.SimpleIntDictionary:
                    for (int iteration = 0; iteration < iterations; iteration++)
                    {
                        for (int i = 0; i < workloadSize; i++)
                        {
                            total += simpleIntValues[i];
                        }
                    }

                    break;

                case LookupVariant.SimpleUIntDictionary:
                    for (int iteration = 0; iteration < iterations; iteration++)
                    {
                        for (int i = 0; i < workloadSize; i++)
                        {
                            total += simpleUIntValues[(uint)i];
                        }
                    }

                    break;

                case LookupVariant.SimpleUlongDictionary:
                    for (int iteration = 0; iteration < iterations; iteration++)
                    {
                        for (int i = 0; i < workloadSize; i++)
                        {
                            total += simpleUlongValues[(ulong)i];
                        }
                    }

                    break;

                default:
                    for (int iteration = 0; iteration < iterations; iteration++)
                    {
                        for (int i = 0; i < workloadSize; i++)
                        {
                            total += simpleStringValues[stringKeys[i]];
                        }
                    }

                    break;
            }

            PerfVisibleSink.Write(total);
        }

        public override void GlobalTeardown()
        {
            dictionaryValues = null;
            dictionaryValuesWithDefaultComparer = null;
            simpleIntValues = null;
            simpleUIntValues = null;
            simpleUlongValues = null;
            simpleStringValues = null;
            stringKeys = null;
        }

        private static string GetCaseName(LookupVariant variant, int workloadSize)
        {
            switch (variant)
            {
                case LookupVariant.DictionaryInt:
                    return "Dictionary_Int_Lookup_" + FormatSizeLabel(workloadSize);
                case LookupVariant.DictionaryIntExplicitDefaultComparer:
                    return "Dictionary_Int_DefaultComparer_Lookup_" + FormatSizeLabel(workloadSize);
                case LookupVariant.SimpleIntDictionary:
                    return "SimpleIntDictionary_Int_Lookup_" + FormatSizeLabel(workloadSize);
                case LookupVariant.SimpleUIntDictionary:
                    return "SimpleUIntDictionary_UInt_Lookup_" + FormatSizeLabel(workloadSize);
                case LookupVariant.SimpleUlongDictionary:
                    return "SimpleUlongDictionary_ULong_Lookup_" + FormatSizeLabel(workloadSize);
                default:
                    return "SimpleStringDictionary_String_Lookup_" + FormatSizeLabel(workloadSize);
            }
        }

        private static string GetCollectionLabel(LookupVariant variant)
        {
            switch (variant)
            {
                case LookupVariant.DictionaryInt:
                    return "Dictionary<int,int>";
                case LookupVariant.DictionaryIntExplicitDefaultComparer:
                    return "Dictionary<int,int>(EqualityComparer<int>.Default)";
                case LookupVariant.SimpleIntDictionary:
                    return "SimpleIntDictionary<int>";
                case LookupVariant.SimpleUIntDictionary:
                    return "SimpleUIntDictionary<int>";
                case LookupVariant.SimpleUlongDictionary:
                    return "SimpleUlongDictionary<int>";
                default:
                    return "SimpleStringDictionary<int>";
            }
        }

        private static string GetVariantLabel(LookupVariant variant)
        {
            switch (variant)
            {
                case LookupVariant.DictionaryInt:
                    return "builtin-int";
                case LookupVariant.DictionaryIntExplicitDefaultComparer:
                    return "builtin-int-explicit-default-comparer";
                case LookupVariant.SimpleIntDictionary:
                    return "simple-int";
                case LookupVariant.SimpleUIntDictionary:
                    return "simple-uint";
                case LookupVariant.SimpleUlongDictionary:
                    return "simple-ulong";
                default:
                    return "simple-string";
            }
        }
    }
}
