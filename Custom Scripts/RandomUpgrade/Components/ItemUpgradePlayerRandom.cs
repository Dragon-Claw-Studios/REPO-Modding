using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RandomUpgrade;
public class ItemUpgradePlayerRandom : MonoBehaviour
{
    private ItemToggle itemToggle;
    private static List<MethodInfo> upgradeMethods;

    private void Start()
    {
        itemToggle = GetComponent<ItemToggle>();

        // Only initialize once
        if (upgradeMethods == null)
        {
            upgradeMethods = typeof(PunManager)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m =>
                    m.Name.StartsWith("UpgradePlayer") &&
                    m.ReturnType == typeof(int) &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(string))
                .ToList();

            Debug.Log($"[ItemUpgradePlayerRandom] Found {upgradeMethods.Count} upgrade methods.");
        }
    }

    public void Upgrade()
    {
        if (upgradeMethods == null || upgradeMethods.Count == 0)
        {
            Debug.LogWarning("[ItemUpgradePlayerRandom] No upgrade methods found.");
            return;
        }

        string steamID = SemiFunc.PlayerGetSteamID(
            SemiFunc.PlayerAvatarGetFromPhotonID(itemToggle.playerTogglePhotonID));

        // Pick a random upgrade method
        MethodInfo method = upgradeMethods[UnityEngine.Random.Range(0, upgradeMethods.Count)];
        Debug.Log($"[ItemUpgradePlayerRandom] Applying upgrade: {method.Name} to SteamID: {steamID}");

        method.Invoke(PunManager.instance, new object[] { steamID });
    }
}
