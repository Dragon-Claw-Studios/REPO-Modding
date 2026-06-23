using UnityEngine;

namespace DragonClawLib
{
    public class MuralBreakInterceptor : MonoBehaviour
    {
        private MuralFragmentController fragments;
        private bool triggered = false;

        private void Awake()
        {
            fragments = GetComponent<MuralFragmentController>();
        }

        // Called automatically by Unity when ANY collider break/destroy starts propagating
        private void OnDestroy()
        {
            TryIntercept();
        }

        private void TryIntercept()
        {
            if (triggered)
                return;

            if (fragments == null)
                return;

            // IMPORTANT: must run only on authority
            if (!SemiFunc.IsMasterClientOrSingleplayer())
                return;

            triggered = true;

            fragments.TryFragment(transform.position);
        }
    }
}