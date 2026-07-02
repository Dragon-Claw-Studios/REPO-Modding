using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ValuableLineupHolder : MonoBehaviour
{
    public float spacing = 1f;

    public enum AxisDirection
    {
        XPositive,
        XNegative,
        ZPositive,
        ZNegative
    }

    public AxisDirection direction = AxisDirection.ZPositive;

    [Header("Optional")]
    public bool includeInactive = true;
    public bool sortByName = true;

    public void RebuildLayout()
    {
        var list = new System.Collections.Generic.List<Transform>();

        foreach (Transform child in transform)
        {
            if (!includeInactive && !child.gameObject.activeSelf)
                continue;

            list.Add(child);
        }

        if (sortByName)
            list.Sort((a, b) => a.name.CompareTo(b.name));

        Vector3 dir = GetDirectionVector(direction);

        for (int i = 0; i < list.Count; i++)
        {
            Transform t = list[i];

            Undo.RecordObject(t, "Rebuild Valuable Layout");

            t.position = transform.position + dir * spacing * i;
        }
    }

    private Vector3 GetDirectionVector(AxisDirection dir)
    {
        switch (dir)
        {
            case AxisDirection.XPositive: return Vector3.right;
            case AxisDirection.XNegative: return Vector3.left;
            case AxisDirection.ZPositive: return Vector3.forward;
            case AxisDirection.ZNegative: return Vector3.back;
        }
        return Vector3.forward;
    }
}