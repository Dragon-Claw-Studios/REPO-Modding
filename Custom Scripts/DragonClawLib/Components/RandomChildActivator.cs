using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace DragonClawLib
{
    public class RandomChildActivator : MonoBehaviourPun
    {
        [Tooltip("If true, allows the possibility that no object is enabled")]
        public bool allowNone = false;

        [Tooltip("If true, pick a random object every time this object enables. Otherwise only once at Start.")]
        public bool randomizeOnEnable = true;

        [Tooltip("Objects that can be randomly activated instead of all children")]
        public List<GameObject> objectsToChooseFrom;

        private void Start()
        {
            if (!randomizeOnEnable && SemiFunc.IsMasterClientOrSingleplayer())
                PickAndSyncRandomObject();
        }

        private void OnEnable()
        {
            if (randomizeOnEnable && SemiFunc.IsMasterClientOrSingleplayer())
                PickAndSyncRandomObject();
        }

        private void PickAndSyncRandomObject()
        {
            if (objectsToChooseFrom.Count == 0 && !allowNone) return;

            int selectedIndex = PickWeightedIndex(objectsToChooseFrom, allowNone);

            // Activate locally
            ActivateObject(selectedIndex);

            // Sync with other clients if in Photon room
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(RPC_ActivateObject), RpcTarget.OthersBuffered, selectedIndex);
            }
        }

        [PunRPC]
        public void RPC_ActivateObject(int selectedIndex)
        {
            ActivateObject(selectedIndex);
        }

        private void ActivateObject(int selectedIndex)
        {
            // "None" option picked
            if (allowNone && selectedIndex == objectsToChooseFrom.Count)
            {
                foreach (var obj in objectsToChooseFrom)
                    if (obj != null) obj.SetActive(false);
                return;
            }

            // Enable selected, disable others
            for (int i = 0; i < objectsToChooseFrom.Count; i++)
            {
                if (objectsToChooseFrom[i] != null)
                    objectsToChooseFrom[i].SetActive(i == selectedIndex);
            }
        }

        private int PickWeightedIndex(List<GameObject> objects, bool allowNoneOption)
        {
            float totalWeight = 0f;
            List<float> weights = new List<float>();

            // Add weights for objects
            foreach (var obj in objects)
            {
                if (obj == null) { weights.Add(0f); continue; }
                var w = obj.GetComponent<WeightedRandom>();
                float weight = w != null ? w.weight : 1f;
                weights.Add(weight);
                totalWeight += weight;
            }

            // Add "none" option
            if (allowNoneOption)
            {
                float noneWeight = 1f;
                weights.Add(noneWeight);
                totalWeight += noneWeight;
            }

            // Pick random number
            float r = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < weights.Count; i++)
            {
                cumulative += weights[i];
                if (r <= cumulative)
                    return i; // last index = "none" if allowNone is true
            }

            return weights.Count - 1; // fallback
        }
    }
}
