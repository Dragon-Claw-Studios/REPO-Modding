using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ValuableLineupHolder))]
public class ValuableLineupHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ValuableLineupHolder holder = (ValuableLineupHolder)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Rebuild Layout"))
        {
            holder.RebuildLayout();
        }
    }
}