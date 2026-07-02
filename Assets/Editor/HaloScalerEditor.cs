using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HaloScaler))]
public class HaloScalerEditor : Editor
{
    private HaloScaler scaler;

    private void OnEnable()
    {
        scaler = (HaloScaler)target;

        // Hook into scene updates
        SceneView.duringSceneGui += OnSceneGUIUpdate;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUIUpdate;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            scaler.Apply();
            EditorUtility.SetDirty(scaler);
        }

        if (GUILayout.Button("Reapply Halo Scaling"))
        {
            scaler.Apply();
        }
    }

    private void OnSceneGUIUpdate(SceneView sceneView)
    {
        if (scaler == null)
            return;

        if (scaler.transform.hasChanged)
        {
            scaler.transform.hasChanged = false;
            scaler.Apply();

            EditorUtility.SetDirty(scaler);
        }
    }
}