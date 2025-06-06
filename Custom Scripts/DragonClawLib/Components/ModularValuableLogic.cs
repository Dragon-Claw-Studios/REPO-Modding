using System.Collections.Generic;
using System.Linq;
using DragonClawLib;
using Photon.Pun;
using UnityEngine;

namespace DragonClawLib
{
    public class ModularValuableLogic : MonoBehaviour
    {
        private ValuableObject valuableObject;
        private PhotonView photonView;

        public List<ValueModifierPart> parts = new List<ValueModifierPart>();

        private void Awake()
        {
            valuableObject = GetComponent<ValuableObject>();
            photonView = GetComponent<PhotonView>();
            // Don't get parts here yet - they may be disabled/not active.
        }

        private void Start()
        {
            // Now that parts should be enabled by ModularValuableBuilder, fetch them:
            parts = GetComponentsInChildren<ValueModifierPart>(true).Where(p => p.gameObject.activeInHierarchy).ToList();

            StartCoroutine(ApplyPartValuesLater());
        }

        private System.Collections.IEnumerator ApplyPartValuesLater()
        {
            yield return new WaitUntil(() => valuableObject.dollarValueSet);

            float baseValue = valuableObject.dollarValueOriginal;
            float modifierTotal = 1f + (0.1f * parts.Sum(part => part.valueModifier));
            float finalValue = Mathf.Round(baseValue * modifierTotal);

            if (SemiFunc.IsMultiplayer())
            {
                if (SemiFunc.IsMasterClient())
                {
                    valuableObject.dollarValueOriginal = finalValue;
                    valuableObject.dollarValueCurrent = finalValue;
                    photonView.RPC("SyncFinalValue", RpcTarget.Others, finalValue);
                }
            }
            else
            {
                valuableObject.dollarValueOriginal = finalValue;
                valuableObject.dollarValueCurrent = finalValue;
            }

            //Debug.Log($"[ModularValuable] Total Value Set: {finalValue} (Base: {baseValue}, Parts: {modifierTotal})");
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
