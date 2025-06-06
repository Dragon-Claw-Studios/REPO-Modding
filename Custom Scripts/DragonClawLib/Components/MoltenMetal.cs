using UnityEngine;

[CreateAssetMenu(fileName = "NewMoltenMetal", menuName = "Phys Object/Molten Metal", order = 2)]
public class MoltenMetal : ScriptableObject
{

    [Header("Value Modification")]
    [Tooltip("Multiplier applied to the object's base value (e.g., 1.1 for +10%)")]
    public float valueMultiplier = 1.1f;

    [Header("Visual Replacement")]
    [Tooltip("Material that replaces the object's opaque parts")]
    public Material castedMaterial;

    [Header("Durability Replacement")]
    [Tooltip("Durability preset to replace the durability")]
    public Durability castedDurability;

    [Header("PhysAttribute Replacement")]
    [Tooltip("PhysAttribute to replace the physics properties like mass")]
    public PhysAttribute castedPhysAttribute;

    [Header("Audio Replacement")]
    [Tooltip("Audio replacement for physical properties")]
    public PhysAudio castedAudioPreset;

    [Header("Impact Particle Gradient Replacement")]
    [Tooltip("Gradient that replaces the particle colors for impacts")]
    public Gradient castedParticleGradient;

}
