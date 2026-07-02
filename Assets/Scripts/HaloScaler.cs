using System;
using System.Collections.Generic;
using UnityEngine;

public class HaloScaler : MonoBehaviour
{
    [Serializable]
    public class HaloEntry
    {
        public Transform haloVisual;
        public float baseScale = 1f;
    }

    [SerializeField]
    private List<HaloEntry> halos = new();

    public void Apply()
    {
        if (halos == null)
            return;

        float scale = Mathf.Max(
            transform.lossyScale.x,
            transform.lossyScale.y,
            transform.lossyScale.z
        );

        foreach (var h in halos)
        {
            if (h.haloVisual == null)
                continue;

            h.haloVisual.localScale = Vector3.one * (h.baseScale * scale);
        }
    }
}