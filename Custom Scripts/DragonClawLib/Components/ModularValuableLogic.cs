using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace DragonClawLib
{
    public class ModularValuableLogic : MonoBehaviour
    {
        private ValuableObject valuableObject;
        private PhotonView photonView;

        private List<StatsModifierPart> parts = new();

        private bool initialized = false;

        private void Awake()
        {
            valuableObject = GetComponent<ValuableObject>();
            photonView = GetComponent<PhotonView>();
        }

        // 🔥 called by builder ONLY when safe
        public void InitializeFromBuilder()
        {
            if (initialized)
                return;

            StartCoroutine(InitializeRoutine());
        }

        private bool HasParticleSystem()
        {
            var impact = GetComponent<PhysGrabObjectImpactDetector>();
            if (impact == null) return false;

            return impact.GetComponentInChildren<PhysObjectParticles>(true) != null;
        }

        private IEnumerator InitializeRoutine()
        {
            yield return null;

            parts = GetComponentsInChildren<StatsModifierPart>(true)
                .Where(p => p.gameObject.activeInHierarchy)
                .ToList();

            ApplyParticleGradientOverride();

            yield return new WaitUntil(() => HasParticleSystem());

            ApplyParticlePatch();
        }

        private void ApplyParticleGradientOverride()
        {
            foreach (var part in parts)
            {
                if (!part.overrideParticleColors)
                    continue;

                if (part.particleColors == null)
                    continue;

                valuableObject.particleColors = part.particleColors;
                break;
            }
        }

        private IEnumerator ApplyPartValuesLater()
        {
            yield return new WaitUntil(() => valuableObject.dollarValueSet);

            float baseValue = valuableObject.dollarValueOriginal;
            float modifierTotal = 1f + (0.1f * parts.Sum(p => p.valueModifier));
            float finalValue = Mathf.Round(baseValue * modifierTotal);

            if (SemiFunc.IsMultiplayer())
            {
                if (SemiFunc.IsMasterClient())
                {
                    valuableObject.dollarValueOriginal = finalValue;
                    valuableObject.dollarValueCurrent = finalValue;

                    photonView.RPC(nameof(SyncFinalValue), RpcTarget.Others, finalValue);
                }
            }
            else
            {
                valuableObject.dollarValueOriginal = finalValue;
                valuableObject.dollarValueCurrent = finalValue;
            }
        }

        private void ApplyParticlePatch()
        {
            var impact = GetComponent<PhysGrabObjectImpactDetector>();
            if (impact == null) return;

            var particles = impact.GetComponentInChildren<PhysObjectParticles>(true);
            if (particles == null) return;

            // 🔥 THIS is the important line
            particles.gradient = valuableObject.particleColors;
        }

        [PunRPC]
        private void SyncFinalValue(float value)
        {
            valuableObject.dollarValueOriginal = value;
            valuableObject.dollarValueCurrent = value;
            valuableObject.dollarValueSet = true;
        }
    }
}