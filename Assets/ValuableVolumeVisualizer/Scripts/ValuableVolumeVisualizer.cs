using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValuableVolumeVisualizer : MonoBehaviour
{    private void OnDrawGizmos()
    {
        BoxCollider component = GetComponent<BoxCollider>();
        Gizmos.color = new Color(1f, 1.18f, 0f, 6f);
        Gizmos.matrix = base.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(component.center, component.size);

        Gizmos.color = new Color(1f, 1.18f, 0f, 0.2f);
        Gizmos.DrawCube(component.center, component.size);

        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.identity;
        Vector3 vector = component.bounds.center;
        Vector3 vector2 = vector + base.transform.forward * 0.5f;
        Gizmos.DrawLine(vector, vector2);
        Gizmos.DrawLine(vector2, vector2 + Vector3.LerpUnclamped(-base.transform.forward, -base.transform.right, 0.5f) * 0.25f);
        Gizmos.DrawLine(vector2, vector2 + Vector3.LerpUnclamped(-base.transform.forward, base.transform.right, 0.5f) * 0.25f);
    }
}
