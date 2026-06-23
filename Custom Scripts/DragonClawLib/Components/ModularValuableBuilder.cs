using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace DragonClawLib
{
    public class ModularValuableBuilder : MonoBehaviourPun
    {
        private Rigidbody rb;

        [Header("Names of child parts (e.g., Valuable_Slot_Blade, Valuable_Slot_Guard, Valuable_Slot_Hilt)")]
        public List<string> partGroupNames = new();

        private bool buildOnAwake = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            if (buildOnAwake && SemiFunc.IsMasterClientOrSingleplayer())
            {
                BuildAndSyncParts();
            }
        }

        private void Start()
        {
            buildOnAwake = false;

            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }

        public void BuildAndSyncParts()
        {
            List<int> selectedIndexes = new();

            foreach (var groupName in partGroupNames)
            {
                Transform group = transform.Find(groupName);

                if (group == null || group.childCount == 0)
                {
                    Debug.LogWarning($"[ModularBuilder] Missing or empty group '{groupName}'");
                    selectedIndexes.Add(-1);
                    continue;
                }

                int selectedIndex = Random.Range(0, group.childCount);
                selectedIndexes.Add(selectedIndex);

                EnableOnly(group, selectedIndex);
            }

            photonView.RPC(nameof(RPC_SyncParts), RpcTarget.OthersBuffered, selectedIndexes.ToArray());

            // 🔥 IMPORTANT: finalize AFTER local build
            FinalizeBuild();
        }

        [PunRPC]
        private void RPC_SyncParts(int[] selectedIndexes)
        {
            for (int i = 0; i < partGroupNames.Count && i < selectedIndexes.Length; i++)
            {
                Transform group = transform.Find(partGroupNames[i]);

                int index = selectedIndexes[i];

                if (group == null || index < 0 || index >= group.childCount)
                    continue;

                EnableOnly(group, index);
            }

            // 🔥 IMPORTANT: finalize AFTER remote build
            FinalizeBuild();
        }

        private void EnableOnly(Transform group, int index)
        {
            foreach (Transform child in group)
                child.gameObject.SetActive(false);

            Transform selected = group.GetChild(index);
            EnableRecursively(selected.gameObject);
        }

        private void EnableRecursively(GameObject obj)
        {
            obj.SetActive(true);

            foreach (Transform child in obj.transform)
                EnableRecursively(child.gameObject);
        }

        // 🔥 THIS is the key to fixing your entire pipeline
        private void FinalizeBuild()
        {
            GetComponent<ModularValuableLogic>()?.InitializeFromBuilder();
        }
    }
}