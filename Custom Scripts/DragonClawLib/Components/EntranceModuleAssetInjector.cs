using System.Collections;
using UnityEngine;

public class EntranceModuleAssetInjector : MonoBehaviour
{
    [Header("Mappings (assign in prefab / bundle)")]
    public RoomOutlineMappingAsset outlineMapping;
    public LevelAmbienceMappingAsset ambienceMapping;

    private bool injected;

    private void Awake()
    {
        // Optional early attempt
        TryInject();
    }

    private IEnumerator Start()
    {
        // Retry until systems exist (handles spawn timing perfectly)
        for (int i = 0; i < 20; i++)
        {
            if (TryInject())
                yield break;

            yield return new WaitForSeconds(0.25f);
        }

        Debug.LogWarning("[EntranceInjector] Failed to inject after retries.");
    }

    private bool TryInject()
    {
        // Prevent double injection per instance
        if (injected)
            return true;

        // Wait for game systems
        if (Map.Instance == null || AudioManager.instance == null)
            return false;

        int outlinesAdded = 0;
        int ambiencesAdded = 0;

        // ---- OUTLINES ----
        if (outlineMapping != null)
        {
            foreach (var o in outlineMapping.customOutlines)
            {
                if (!Map.Instance.RoomVolumeOutlineCustoms.Contains(o))
                {
                    Map.Instance.RoomVolumeOutlineCustoms.Add(o);
                    outlinesAdded++;
                }
            }
        }

        // ---- AMBIENCES ----
        if (ambienceMapping != null)
        {
            foreach (var a in ambienceMapping.customAmbiences)
            {
                if (!AudioManager.instance.levelAmbiences.Contains(a))
                {
                    AudioManager.instance.levelAmbiences.Add(a);
                    ambiencesAdded++;
                }
            }
        }

        injected = true;

        Debug.Log($"[EntranceInjector] Injected {outlinesAdded} outlines, {ambiencesAdded} ambiences");

        return true;
    }
}