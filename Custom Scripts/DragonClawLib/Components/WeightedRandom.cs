using UnityEngine;

namespace DragonClawLib
{
    public class WeightedRandom : MonoBehaviour
    {
        [Range(0.05f, 1f)]
        public float weight = 1f; // Default 1 = normal chance
    }
}