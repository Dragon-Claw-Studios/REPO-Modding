using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DragonClawLib
{
    public class MuralFragmentController : MonoBehaviour
    {
        [Header("Fragment pieces (disabled children under mural)")]
        public List<GameObject> fragments = new();

        [Header("Spawn rules")]
        public int minPieces = 2;
        public int maxPieces = 5;

        public bool hasFragmented = false;

        public void TryFragment(Vector3 hitPoint)
        {
            if (hasFragmented)
                return;

            hasFragmented = true;

            int count = Random.Range(minPieces, maxPieces + 1);

            var pool = fragments.Where(f => f != null).ToList();
            Shuffle(pool);

            for (int i = 0; i < Mathf.Min(count, pool.Count); i++)
            {
                var obj = pool[i];

                obj.transform.SetParent(null, true);

                obj.SetActive(true);

                /*
                if (obj.TryGetComponent<Rigidbody>(out var rb))
                {
                    Vector3 dir = (obj.transform.position - hitPoint).normalized;
                    rb.AddForce(dir * Random.Range(1f, 3f), ForceMode.Impulse);
                }
                */
            }
        }

        private void Shuffle(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int r = Random.Range(i, list.Count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}