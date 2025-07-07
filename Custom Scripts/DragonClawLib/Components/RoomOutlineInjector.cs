using System.Collections;
using UnityEngine;

public static class RoomOutlineInjector
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        CoroutineRunner.Run(WaitForMapAndInject());
    }

    private static IEnumerator WaitForMapAndInject()
    {
        // Wait until Map.Instance is assigned
        while (Map.Instance == null)
        {
            yield return null;
        }


        // Load all assets from Resources folder "RoomOutlineMappings"
        var customAssets = Resources.LoadAll<RoomOutlineMappingAsset>("ScriptableObjects/RoomOutlineMappings");

        int totalInjected = 0;

        foreach (var asset in customAssets)
        {

            Map.Instance.RoomVolumeOutlineCustoms.AddRange(asset.customOutlines);
            totalInjected += asset.customOutlines.Count;
        }

        //Debug.Log($"Injected {totalInjected} custom room outlines from {customAssets.Length} assets.");
    }
}
