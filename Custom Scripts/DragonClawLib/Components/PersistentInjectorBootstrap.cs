using System.Collections;
using UnityEngine;

public class PersistentInjectorBootstrap : MonoBehaviour
{
    private static bool created = false;
    private static PersistentInjectorBootstrap instance;
    private bool injectedThisScene = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (created) return;
        var go = new GameObject("PersistentInjector");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<PersistentInjectorBootstrap>();
        created = true;
        Debug.Log("[InjectorBootstrap] Persistent injector created.");
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
    {
        injectedThisScene = false;
        StartCoroutine(WaitAndInjectRoutine());
    }

    private IEnumerator WaitAndInjectRoutine()
    {
        // Wait a bit to ensure managers have reinitialized
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 10; i++) // Try for ~10 * 0.5s = 5s
        {
            if (AudioManager.instance != null && Map.Instance != null)
            {
                yield return Inject();
                injectedThisScene = true;
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }

        Debug.LogWarning("[InjectorBootstrap] Timeout waiting for systems to initialize.");
    }

    private IEnumerator Inject()
    {
        yield return null; // Wait one frame

        var outlines = Resources.LoadAll<RoomOutlineMappingAsset>("ScriptableObjects/RoomOutlineMappings");
        var ambiences = Resources.LoadAll<LevelAmbienceMappingAsset>("ScriptableObjects/LevelAmbiences");

        if (AudioManager.instance == null)
        {
            Debug.LogError("[InjectorBootstrap] AudioManager.instance missing during injection!");
            yield break;
        }

        if (Map.Instance == null)
        {
            Debug.LogError("[InjectorBootstrap] Map.Instance missing during injection!");
            yield break;
        }

        int totalOutlines = 0, totalAmbiences = 0;

        foreach (var asset in outlines)
        {
            if (asset.customOutlines != null)
            {
                Map.Instance.RoomVolumeOutlineCustoms.AddRange(asset.customOutlines);
                totalOutlines += asset.customOutlines.Count;
            }
        }

        foreach (var asset in ambiences)
        {
            if (asset.customAmbiences != null)
            {
                AudioManager.instance.levelAmbiences.AddRange(asset.customAmbiences);
                totalAmbiences += asset.customAmbiences.Count;
            }
        }

        Debug.Log($"[InjectorBootstrap] Injected {totalOutlines} outlines and {totalAmbiences} ambiences.");
    }
}
