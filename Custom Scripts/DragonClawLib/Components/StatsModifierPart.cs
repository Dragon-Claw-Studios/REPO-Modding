using UnityEngine;

public class StatsModifierPart : MonoBehaviour
{
    [Header("Value")]
    [Tooltip("Percentage that this part adds to the base value of the item.")]
    public float valueModifier = 0f;

    [Header("Particle Colors")]

    [Tooltip("Should this part override the particle colors?")]
    public bool overrideParticleColors = false;

    [Tooltip("Particle colors to use if overriding.")]
    public Gradient particleColors;
}