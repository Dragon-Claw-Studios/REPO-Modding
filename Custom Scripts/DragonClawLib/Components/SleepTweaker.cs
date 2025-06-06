using UnityEngine;

namespace DragonClawLib;
[RequireComponent(typeof(Rigidbody))]
public class SleepTweaker : MonoBehaviour
{
    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 0.2f; // Higher = easier to sleep, less jitter
    }
}
