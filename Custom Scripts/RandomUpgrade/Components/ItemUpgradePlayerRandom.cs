using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Photon.Pun;

namespace RandomUpgrade
{
    public class ItemUpgradePlayerRandom : MonoBehaviour
    {
        private ItemToggle _itemToggle;
        private static List<MethodInfo> _upgradeMethods;

        private void Start()
        {
            _itemToggle = GetComponent<ItemToggle>();

            // Initialize methods only once
            if (_upgradeMethods == null)
            {
                _upgradeMethods = typeof(PunManager)
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m =>
                        m.Name.StartsWith("UpgradePlayer", StringComparison.Ordinal) &&
                        m.ReturnType == typeof(int) &&
                        m.GetParameters().Length == 1 &&
                        m.GetParameters()[0].ParameterType == typeof(string))
                    .ToList();
            }
        }

        public void Upgrade()
        {
            // Only allow the master client to perform the upgrade
            if (!PhotonNetwork.IsMasterClient) return;

            // Ensure there are available methods
            if (_upgradeMethods == null || _upgradeMethods.Count == 0) return;

            string steamId = SemiFunc.PlayerGetSteamID(
                SemiFunc.PlayerAvatarGetFromPhotonID(_itemToggle.playerTogglePhotonID));

            // Pick a random upgrade method and invoke
            MethodInfo method = _upgradeMethods[UnityEngine.Random.Range(0, _upgradeMethods.Count)];
            method.Invoke(PunManager.instance, new object[] { steamId });
        }
    }
}
