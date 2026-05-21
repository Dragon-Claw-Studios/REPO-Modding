using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingTray : MonoBehaviour
{
    [Header("Allowed Volume Types")]
    public List<ValuableVolume.Type> allowedVolumeTypes = new List<ValuableVolume.Type>();

    public CastingPot castingPot;
    public string materialNameSkip;
    public GameObject hurtCollider;

    public List<ValuableObject> containedValuables = new List<ValuableObject>();
    private Dictionary<ValuableObject, int> colliderCounts = new Dictionary<ValuableObject, int>();

    [Header("Pour Visuals")]
    public List<GameObject> pourVisuals;
    public Transform liquidLayer;
    public float pourDepth = 0.5f;
    public float pourDuration = 1.5f;

    private Material instancedLiquidMaterial;
    private Material liquidLayerMaterialInstance;
    private Renderer liquidLayerRenderer;

    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private string transparencyProperty = "_Transparency";

    [Header("Indicator Lamps")]
    public List<Renderer> indicatorLampRenderers = new List<Renderer>();

    public Color redEmission = Color.red;
    public Color orangeEmission = new Color(1f, 0.5f, 0f);
    public Color greenEmission = Color.green;

    private Color lastAppliedEmission = Color.clear;

    // One instanced material per renderer
    private List<Material> indicatorMaterialInstances = new List<Material>();

    void Start()
    {
        InitializeIndicatorLamps();
        UpdateIndicatorColor();
        PourVisualsInitialize();
    }

    void InitializeIndicatorLamps()
    {
        indicatorMaterialInstances.Clear();

        foreach (var renderer in indicatorLampRenderers)
        {
            if (renderer == null)
                continue;

            Material instance = renderer.material;
            indicatorMaterialInstances.Add(instance);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var valuable = other.GetComponentInParent<ValuableObject>();
        if (valuable == null) return;

        // Volume type filter
        if (allowedVolumeTypes.Count > 0 && !allowedVolumeTypes.Contains(valuable.volumeType))
        {
            return;
        }

        var renderers = valuable.GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat == null) continue;

                if (mat.name.StartsWith(materialNameSkip))
                {
                    return;
                }
            }
        }

        if (!colliderCounts.ContainsKey(valuable))
            colliderCounts[valuable] = 0;

        colliderCounts[valuable]++;

        if (!containedValuables.Contains(valuable))
        {
            containedValuables.Add(valuable);

            var watcher = valuable.GetComponent<ValuableDestructionWatcher>();

            if (watcher == null)
            {
                watcher = valuable.gameObject.AddComponent<ValuableDestructionWatcher>();
            }

            if (watcher.tray != this)
            {
                watcher.tray = this;
            }

            watcher.valuable = valuable;
        }

        UpdateIndicatorColor();
    }

    void OnTriggerExit(Collider other)
    {
        var valuable = other.GetComponentInParent<ValuableObject>();

        if (valuable == null || !colliderCounts.ContainsKey(valuable))
            return;

        colliderCounts[valuable]--;

        if (colliderCounts[valuable] <= 0)
        {
            containedValuables.Remove(valuable);
            colliderCounts.Remove(valuable);

            UpdateIndicatorColor();
        }
    }

    public void OnValuableDestroyed(ValuableObject valuable)
    {
        containedValuables.Remove(valuable);
        colliderCounts.Remove(valuable);

        UpdateIndicatorColor();
    }

    public void UpdateIndicatorColor()
    {
        if (indicatorMaterialInstances.Count == 0)
            return;

        Color targetColor;

        if (castingPot != null && castingPot.hasPoured)
        {
            targetColor = Color.black;
        }
        else if (containedValuables.Count == 0)
        {
            targetColor = redEmission;
        }
        else if (containedValuables.Count == 1)
        {
            targetColor = greenEmission;
        }
        else
        {
            targetColor = orangeEmission;
        }

        if (targetColor == lastAppliedEmission)
            return;

        lastAppliedEmission = targetColor;

        foreach (var material in indicatorMaterialInstances)
        {
            if (material == null)
                continue;

            material.SetColor("_Color", targetColor == Color.black ? Color.white : targetColor);
            material.SetColor("_EmissionColor", targetColor);
        }
    }

    public void ApplyCastingToAll(MoltenMetal metal)
    {
        foreach (var valuable in containedValuables)
        {
            ApplyMoltenEffect(valuable, metal);
            UpdateImpactDetector(valuable);
        }
    }

    void ApplyMoltenEffect(ValuableObject obj, MoltenMetal moltenMetalPreset)
    {
        var renderers = obj.GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            if (!renderer.enabled)
            {
                continue;
            }

            Material[] newMats = new Material[renderer.sharedMaterials.Length];

            for (int i = 0; i < newMats.Length; i++)
            {
                Material original = renderer.sharedMaterials[i];

                if (original == null)
                {
                    newMats[i] = null;
                    continue;
                }

                Material moltenCopy = new Material(original);

                moltenCopy.name = moltenMetalPreset.castedMaterial.name;
                moltenCopy.shader = moltenMetalPreset.castedMaterial.shader;

                moltenCopy.SetFloat("_Overlay_Albedo_Intensity", moltenMetalPreset.castedMaterial.GetFloat("_Overlay_Albedo_Intensity"));
                moltenCopy.SetFloat("_Overlay_Effects_Intensity", moltenMetalPreset.castedMaterial.GetFloat("_Overlay_Effects_Intensity"));
                moltenCopy.SetTexture("_Overlay_Albedo", moltenMetalPreset.castedMaterial.GetTexture("_Overlay_Albedo"));
                moltenCopy.SetColor("_Overlay_Color", moltenMetalPreset.castedMaterial.GetColor("_Overlay_Color"));
                moltenCopy.SetTexture("_Overlay_Metallic", moltenMetalPreset.castedMaterial.GetTexture("_Overlay_Metallic"));
                moltenCopy.SetTexture("_Overlay_Normal", moltenMetalPreset.castedMaterial.GetTexture("_Overlay_Normal"));
                moltenCopy.SetTexture("_Overlay_Roughness", moltenMetalPreset.castedMaterial.GetTexture("_Overlay_Roughness"));

                if (original.HasProperty("_Metallic"))
                {
                    if (original.GetFloat("_Metallic") > moltenCopy.GetFloat("_Metallic"))
                    {
                        moltenCopy.SetFloat("_Metallic", original.GetFloat("_Metallic"));
                    }
                    else
                    {
                        moltenCopy.SetFloat("_Metallic", moltenMetalPreset.castedMaterial.GetFloat("_Metallic"));
                    }
                }

                if (original.HasProperty("_BumpScale"))
                {
                    if (original.GetFloat("_BumpScale") > moltenCopy.GetFloat("_BumpScale"))
                    {
                        moltenCopy.SetFloat("_BumpScale", original.GetFloat("_BumpScale"));
                    }
                    else
                    {
                        moltenCopy.SetFloat("_BumpScale", moltenMetalPreset.castedMaterial.GetFloat("_BumpScale"));
                    }
                }

                if (original.HasProperty("_GlossMapScale"))
                {
                    if (original.GetFloat("_GlossMapScale") > moltenCopy.GetFloat("_GlossMapScale"))
                    {
                        moltenCopy.SetFloat("_GlossMapScale", original.GetFloat("_GlossMapScale"));
                    }
                    else
                    {
                        moltenCopy.SetFloat("_GlossMapScale", moltenMetalPreset.castedMaterial.GetFloat("_GlossMapScale"));
                    }
                }

                newMats[i] = moltenCopy;
            }

            renderer.materials = newMats;
        }

        obj.dollarValueCurrent *= moltenMetalPreset.valueMultiplier;
        obj.durabilityPreset = moltenMetalPreset.castedDurability;
        obj.audioPreset = moltenMetalPreset.castedAudioPreset;
        obj.particleColors = moltenMetalPreset.castedParticleGradient;

        if (obj.physAttributePreset.mass < moltenMetalPreset.castedPhysAttribute.mass)
        {
            obj.physAttributePreset = moltenMetalPreset.castedPhysAttribute;
            obj.gameObject.GetComponent<Rigidbody>().mass = obj.physAttributePreset.mass;
        }
    }

    void UpdateImpactDetector(ValuableObject valuable)
    {
        var detector = valuable.GetComponent<PhysGrabObjectImpactDetector>();

        if (detector == null)
            return;

        detector.durability = valuable.durabilityPreset.durability;
        detector.fragility = valuable.durabilityPreset.fragility;

        detector.impactAudio = valuable.audioPreset;
        detector.impactAudioPitch = valuable.audioPresetPitch;

        if (detector.particles != null)
        {
            detector.particles.gradient = valuable.particleColors;
        }
    }

    public void DisableHurtCollider()
    {
        hurtCollider.SetActive(false);
    }

    public void PlayPouringVisuals()
    {
        StartCoroutine(PourVisualCoroutine());
    }

    private void PourVisualsInitialize()
    {
        // Initialize pour visuals
        foreach (var obj in pourVisuals)
        {
            if (obj != null && obj.TryGetComponent<Renderer>(out var renderer))
            {
                if (instancedLiquidMaterial == null)
                {
                    instancedLiquidMaterial = new Material(renderer.sharedMaterial);
                }

                renderer.material = instancedLiquidMaterial;
            }

            if (obj != null)
                obj.SetActive(false);
        }

        // Initialize liquid layer material separately
        if (liquidLayer != null)
        {
            liquidLayerRenderer = liquidLayer.GetComponent<Renderer>();

            if (liquidLayerRenderer != null)
            {
                liquidLayerMaterialInstance = new Material(liquidLayerRenderer.sharedMaterial);
                liquidLayerRenderer.material = liquidLayerMaterialInstance;
            }
        }
    }

    private IEnumerator PourVisualCoroutine()
    {
        foreach (var obj in pourVisuals)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (instancedLiquidMaterial != null)
        {
            float tFadeIn = 0f;

            while (tFadeIn < fadeDuration)
            {
                tFadeIn += Time.deltaTime;

                float alpha = Mathf.Lerp(0f, 1f, tFadeIn / fadeDuration);

                instancedLiquidMaterial.SetFloat(transparencyProperty, alpha);

                yield return null;
            }

            instancedLiquidMaterial.SetFloat(transparencyProperty, 1f);
        }

        // --- Pour Movement + Simultaneous Transparency Fade ---
        if (liquidLayer != null)
        {
            Vector3 start = liquidLayer.localPosition;
            Vector3 target = start - new Vector3(0, pourDepth, 0);

            float t = 0f;

            while (t < pourDuration)
            {
                t += Time.deltaTime;

                float progress = Mathf.Clamp01(t / pourDuration);

                // Move liquid downward
                liquidLayer.localPosition = Vector3.Lerp(start, target, progress);

                // Fade pour stream material
                if (instancedLiquidMaterial != null)
                {
                    float alpha = Mathf.Lerp(1f, 0f, progress);
                    instancedLiquidMaterial.SetFloat(transparencyProperty, alpha);
                }

                // Fade liquid layer material
                if (liquidLayerMaterialInstance != null)
                {
                    float alpha = Mathf.Lerp(1f, 0f, progress);
                    liquidLayerMaterialInstance.SetFloat(transparencyProperty, alpha);
                }

                yield return null;
            }

            liquidLayer.localPosition = target;

            if (instancedLiquidMaterial != null)
            {
                instancedLiquidMaterial.SetFloat(transparencyProperty, 0f);
            }

            if (liquidLayerMaterialInstance != null)
            {
                liquidLayerMaterialInstance.SetFloat(transparencyProperty, 0f);
            }
        }

        yield return new WaitForSeconds(1f);

        foreach (var obj in pourVisuals)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}