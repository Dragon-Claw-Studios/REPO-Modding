using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CastingTray : MonoBehaviour
{
    public CastingPot castingPot;
    public string materialNameSkip;
    public GameObject hurtCollider;

    public List<ValuableObject> containedValuables = new List<ValuableObject>();
    private Dictionary<ValuableObject, int> colliderCounts = new Dictionary<ValuableObject, int>();

    [Header("Pour Visuals")]
    public List<GameObject> pourVisuals;
    public Transform liquidLayer;         // Visual part of the liquid
    public float pourDepth = 0.5f;        // How far liquid sinks
    public float pourDuration = 1.5f;     // Time for the liquid to animate

    private Material instancedLiquidMaterial;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private string transparencyProperty = "_Transparency";

    [Header("Indicator Lamp")]
    public Renderer indicatorLampRenderer;
    public Color redEmission = Color.red;
    public Color orangeEmission = new Color(1f, 0.5f, 0f); // Bright orange
    public Color greenEmission = Color.green;
    private Color lastAppliedEmission = Color.clear;

    private Material indicatorMaterialInstance;

    void Start()
    {
        if (indicatorLampRenderer != null)
        {
            indicatorMaterialInstance = indicatorLampRenderer.material;
            UpdateIndicatorColor(); 
            PourVisualsInitialize(); 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var valuable = other.GetComponentInParent<ValuableObject>();
        if (valuable == null) return;

        var renderers = valuable.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat.name.StartsWith(materialNameSkip))
                {
                    // Debug.Log($"Skipping {valuable.name}, already cast.");
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
            // Debug.Log($"Adding item to be cast: {valuable.name}");

            // Attach the destruction watcher
            var watcher = valuable.gameObject.AddComponent<ValuableDestructionWatcher>();
            watcher.tray = this;
            watcher.valuable = valuable;
        }
        UpdateIndicatorColor();
    }





    void OnTriggerExit(Collider other)
    {
        var valuable = other.GetComponentInParent<ValuableObject>();
        if (valuable == null || !colliderCounts.ContainsKey(valuable)) return;

        colliderCounts[valuable]--;

        if (colliderCounts[valuable] <= 0)
        {
            // Debug.Log($"Removing item to be cast: {valuable.name}");
            containedValuables.Remove(valuable);
            colliderCounts.Remove(valuable);
            UpdateIndicatorColor();
        }
    }

    public void OnValuableDestroyed(ValuableObject valuable)
    {
        containedValuables.Remove(valuable);
        colliderCounts.Remove(valuable);
        // Debug.Log($"Destroyed valuable removed from tray: {valuable.name}");

        UpdateIndicatorColor();
    }

    public void UpdateIndicatorColor()
    {
        if (indicatorMaterialInstance == null) return;

        Color targetColor;

        if (castingPot != null && castingPot.hasPoured)
        {
            targetColor = Color.black;
        }
        else if (containedValuables.Count == 0)
            targetColor = redEmission;
        else if (containedValuables.Count == 1)
            targetColor = greenEmission;
        else
            targetColor = orangeEmission;

        if (targetColor == lastAppliedEmission) return; // No need to update

        lastAppliedEmission = targetColor;

        indicatorMaterialInstance.SetColor("_Color", targetColor == Color.black ? Color.white : targetColor);
        indicatorMaterialInstance.SetColor("_EmissionColor", targetColor);
    }

    public void ApplyCastingToAll(MoltenMetal metal)
    {
        Debug.Log("Applying molten effect");
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
                // Skip renderers used only for colliders or invisible meshes
                continue;
            }

            Material[] newMats = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < newMats.Length; i++)
            {
                Material original = renderer.sharedMaterials[i];

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

        // Modify stats
        obj.dollarValueCurrent *= moltenMetalPreset.valueMultiplier;
        obj.durabilityPreset = moltenMetalPreset.castedDurability;
        obj.audioPreset = moltenMetalPreset.castedAudioPreset;
        obj.particleColors = moltenMetalPreset.castedParticleGradient;

        if (obj.physAttributePreset.mass < moltenMetalPreset.castedPhysAttribute.mass)
        {
            // Retain original mass if object is "heavier" than the cast metal properties
            obj.physAttributePreset = moltenMetalPreset.castedPhysAttribute;
            obj.gameObject.GetComponent<Rigidbody>().mass = obj.physAttributePreset.mass;
        }

        // Debug.Log($"Casted {obj.name} with {moltenMetalPreset.name}");
    }


    void UpdateImpactDetector(ValuableObject valuable)
    {
        var detector = valuable.GetComponent<PhysGrabObjectImpactDetector>();
        if (detector == null) return;

        // Update durability and fragility
        detector.durability = valuable.durabilityPreset.durability;
        detector.fragility = valuable.durabilityPreset.fragility;

        // Update audio
        detector.impactAudio = valuable.audioPreset;
        detector.impactAudioPitch = valuable.audioPresetPitch;

        // update gradient for particles if used
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
        // Clone the material once from the first pourVisual with a Renderer
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

            // Ensure visuals start disabled
            if (obj != null) obj.SetActive(false);
        }
    }


    private IEnumerator PourVisualCoroutine()
    {
        // Activate VFX objects (e.g. pour stream)
        foreach (var obj in pourVisuals)
        {
            if (obj != null) obj.SetActive(true);
        }

        // --- Fade In Transparency (0 → 1) ---
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

        // --- Pour Movement ---
        if (liquidLayer != null)
        {
            Vector3 start = liquidLayer.localPosition;
            Vector3 target = start - new Vector3(0, pourDepth, 0);
            float t = 0f;

            while (t < pourDuration)
            {
                t += Time.deltaTime;
                float progress = Mathf.Clamp01(t / pourDuration);
                liquidLayer.localPosition = Vector3.Lerp(start, target, progress);
                yield return null;
            }

            liquidLayer.localPosition = target;
        }

        // --- Fade Out Transparency (1 → 0) ---
        if (instancedLiquidMaterial != null)
        {
            float tFadeOut = 0f;
            while (tFadeOut < fadeDuration)
            {
                tFadeOut += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, tFadeOut / fadeDuration);
                instancedLiquidMaterial.SetFloat(transparencyProperty, alpha);
                yield return null;
            }
            instancedLiquidMaterial.SetFloat(transparencyProperty, 0f);
        }

        yield return new WaitForSeconds(1f);

        foreach (var obj in pourVisuals)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

}
