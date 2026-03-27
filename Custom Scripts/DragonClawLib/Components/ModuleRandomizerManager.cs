using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

namespace DragonClawLib
{
    [RequireComponent(typeof(PhotonView))]
    public class ModuleRandomizerManager : MonoBehaviourPun
    {
        private List<RandomChildActivator> randomizers = new List<RandomChildActivator>();

        private void Awake()
        {
            randomizers = GetComponentsInChildren<RandomChildActivator>(true).ToList();

            // Sort by hierarchy path for deterministic ordering
            randomizers = randomizers
                .OrderBy(r => GetHierarchyPath(r.transform))
                .ToList();
        }

        private void Start()
        {
            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                RandomizeAll();
            }
        }

        /// <summary>
        /// Randomizes all child randomizers and syncs via parent PhotonView
        /// </summary>
        public void RandomizeAll()
        {
            if (randomizers.Count == 0) return;

            int[] results = new int[randomizers.Count];

            for (int i = 0; i < randomizers.Count; i++)
            {
                int index = randomizers[i].PickRandomIndex();
                results[i] = index;

                randomizers[i].ActivateObject(index);
            }

            // Sync across network
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(RPC_ApplyResults), RpcTarget.OthersBuffered, results);
            }
        }

        [PunRPC]
        private void RPC_ApplyResults(int[] results)
        {
            for (int i = 0; i < randomizers.Count && i < results.Length; i++)
            {
                randomizers[i].ActivateObject(results[i]);
            }
        }

        // ------------------------
        // Helpers
        // ------------------------
        private string GetHierarchyPath(Transform t)
        {
            string path = t.name;

            while (t.parent != null && t.parent != transform)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }

            return path;
        }
    }
}