using System.Collections.Generic;
using System.Linq;
using DragonClawLib;
using Photon.Pun;
using UnityEngine;

namespace DragonClawLib;
public class ModularValuableLogic : MonoBehaviour
{
    private ValuableObject valuableObject;
    private PhotonView photonView;

    public List<ValueModifierPart> parts = new List<ValueModifierPart>();

    private void Awake()
    {
        valuableObject = GetComponent<ValuableObject>();
        photonView = GetComponent<PhotonView>();

        // Auto-find all parts on children if not assigned manually
        if (parts.Count == 0)
            parts = GetComponentsInChildren<ValueModifierPart>().ToList();
    }

    private void Start()
    {
        // Wait for ValuableObject to start coroutine first
        StartCoroutine(ApplyPartValuesLater());
    }

    private System.Collections.IEnumerator ApplyPartValuesLater()
    {
        // Wait until dollar value is initially set
        yield return new WaitUntil(() => valuableObject.dollarValueSet);

        float baseValue = valuableObject.dollarValueOriginal;
        float modifierTotal = parts.Sum(part => part.valueModifier);
        float finalValue = Mathf.Round(baseValue + modifierTotal);

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

        Debug.Log($"[ModularValuable] Total Value Set: {finalValue} (Base: {baseValue}, Parts: {modifierTotal})");
    }

    [PunRPC]
    private void SyncFinalValue(float value)
    {
        valuableObject.dollarValueOriginal = value;
        valuableObject.dollarValueCurrent = value;
        valuableObject.dollarValueSet = true;
    }
}
