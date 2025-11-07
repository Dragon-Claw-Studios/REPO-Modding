using System.Collections;
using UnityEngine;

public class BuildSafeInjectorBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        // Create a persistent GameObject to run the injector
        GameObject go = new GameObject("BuildSafeInjector");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<BuildSafeInjectorBootstrap>();
    }

    private void Start()
    {
        StartCoroutine(InjectRoutine());
    }

    private IEnumerator InjectRoutine()
    {
        // Wait until Map and AudioManager exist
        while (Map.Instance == null || AudioManager.instance == null)
            yield return null;

        // Inject Room Outlines
        foreach (var asset in Resources.LoadAll<RoomOutlineMappingAsset>("ScriptableObjects/RoomOutlineMappings"))
            Map.Instance.RoomVolumeOutlineCustoms.AddRange(asset.customOutlines);

        // Inject Ambience
        foreach (var asset in Resources.LoadAll<LevelAmbienceMappingAsset>("ScriptableObjects/LevelAmbiences"))
            AudioManager.instance.levelAmbiences.AddRange(asset.customAmbiences);

        Debug.Log("Custom assets injected!");
    }
}
