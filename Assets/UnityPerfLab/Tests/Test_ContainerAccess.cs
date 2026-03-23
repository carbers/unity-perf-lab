using Perf.Util;
using UnityEngine;

namespace UnityPerfLab.Tests
{
    /// <summary>
    /// Verification component that confirms the perf.util package is imported
    /// and core container classes are accessible.
    /// </summary>
    public class Test_ContainerAccess : MonoBehaviour
    {
        private void Start()
        {
            var simpleList = new SimpleList<int>();
            simpleList.Add(42);
            Debug.Log($"SimpleList<int> instantiated and item added. Count: {simpleList.Count}");

            var intDict = new SimpleIntDictionary<int>();
            intDict.Add(1, 100);
            Debug.Log($"SimpleIntDictionary<int> instantiated and item added. Value at key 1: {intDict[1]}");

            Debug.Log("All perf.util container classes are accessible and functional.");
        }
    }
}
