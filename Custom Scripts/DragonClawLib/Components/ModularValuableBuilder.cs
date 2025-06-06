using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
                BuildAndSyncParts();



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

        // Called by master client only
        public void BuildAndSyncParts()
        {
            List<int> selectedIndexes = new();

            foreach (var groupName in partGroupNames)
            {
                Transform group = transform.Find(groupName);
                if (group == null || group.childCount == 0)
                {
                    Debug.LogWarning($"[ModularBuilder] Missing or empty group '{groupName}'");
                    selectedIndexes.Add(-1); // sentinel for missing group
                    continue;
                }

                int selectedIndex = Random.Range(0, group.childCount);
                selectedIndexes.Add(selectedIndex);

                EnableOnly(group, selectedIndex);
            }

            // Sync with others
            photonView.RPC(nameof(RPC_SyncParts), RpcTarget.OthersBuffered, selectedIndexes.ToArray());
        }

        // RPC to sync enabled parts
        [PunRPC]
        private void RPC_SyncParts(int[] selectedIndexes)
        {
            for (int i = 0; i < partGroupNames.Count && i < selectedIndexes.Length; i++)
            {
                string groupName = partGroupNames[i];
                int selectedIndex = selectedIndexes[i];

                Transform group = transform.Find(groupName);
                if (group == null || selectedIndex < 0 || selectedIndex >= group.childCount)
                    continue;

                EnableOnly(group, selectedIndex);
            }
        }

        private void EnableOnly(Transform group, int index)
        {
            // Disable all first
            foreach (Transform child in group)
                child.gameObject.SetActive(false);

            // Enable selected
            Transform selected = group.GetChild(index);
            EnableRecursively(selected.gameObject);
        }

        private void EnableRecursively(GameObject obj)
        {
            obj.SetActive(true);
            foreach (Transform child in obj.transform)
            {
                EnableRecursively(child.gameObject);
            }
        }
    }
}
