using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace DragonClawLib
{
    public class RandomChildActivator : MonoBehaviour
    {
        [Tooltip("If true, allows the possibility that no object is enabled")]
        public bool allowNone = false;

        [Tooltip("Objects that can be randomly activated instead of all children")]
        public List<GameObject> objectsToChooseFrom;

        /// <summary>
        /// Picks a random index locally without syncing
        /// </summary>
        public int PickRandomIndex()
        {
            if (objectsToChooseFrom.Count == 0 && !allowNone) return -1;

            float totalWeight = 0f;
            List<float> weights = new List<float>();

            foreach (var obj in objectsToChooseFrom)
            {
                if (obj == null) { weights.Add(0f); continue; }
                var w = obj.GetComponent<WeightedRandom>();
                float weight = w != null ? w.weight : 1f;
                weights.Add(weight);
                totalWeight += weight;
            }

            if (allowNone)
            {
                float noneWeight = 1f;
                weights.Add(noneWeight);
                totalWeight += noneWeight;
            }

            float r = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < weights.Count; i++)
            {
                cumulative += weights[i];
                if (r <= cumulative)
                    return i;
            }

            return weights.Count - 1;
        }

        /// <summary>
        /// Activates the child corresponding to selectedIndex. Handles "none" option.
        /// No PhotonViews are involved.
        /// </summary>
        public void ActivateObject(int selectedIndex)
        {
            for (int i = 0; i < objectsToChooseFrom.Count; i++)
            {
                GameObject obj = objectsToChooseFrom[i];
                if (obj == null) continue;

                bool shouldBeActive = (i == selectedIndex);
                obj.SetActive(shouldBeActive);
            }

            // Handle "none" option
            if (allowNone && selectedIndex == objectsToChooseFrom.Count)
            {
                foreach (var obj in objectsToChooseFrom)
                    if (obj != null) obj.SetActive(false);
            }
        }
    }
}