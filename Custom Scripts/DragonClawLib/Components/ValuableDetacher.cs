using UnityEngine;

namespace DragonClawLib
{
    public class ValuableDetacher : MonoBehaviour
    {
        [Tooltip("Name of the decorative object (e.g., Sword_Rack_01) to detach at runtime")]
        public string detachableName = "Holder";

        private Rigidbody rb;
        private Transform rack;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll; // Lock in place temporarily
            }

            rack = transform.Find(detachableName);
            if (rack != null)
            {
                rack.SetParent(null); // Detach from the valuable
                Debug.Log($"[ValuableDetacher] Detached '{detachableName}' from valuable");
            }
            else
            {
                Debug.LogWarning($"[ValuableDetacher] Could not find child '{detachableName}'");
            }
        }

        private void Start()
        {
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.None; // Let physics take over
                Debug.Log("[ValuableDetacher] Rigidbody constraints released");
            }
        }
    }
}
