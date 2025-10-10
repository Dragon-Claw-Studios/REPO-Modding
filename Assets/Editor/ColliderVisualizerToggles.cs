using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ColliderVisualizerToggles
{
    private const string Key_InvisibleCollider = "InvisibleColliderVisualizerToggle";
    private const string Key_GhostMesh = "GhostMeshVisualizerToggle";
    private const string Key_Valuables = "ValuablesVisualizerToggle";
    private const string Key_Paths = "PathsVisualizerToggle";

    static ColliderVisualizerToggles()
    {
        LoadPrefs();
        // Subscribe to playmode changes
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void LoadPrefs()
    {
        InvisibleCollider = EditorPrefs.GetBool(Key_InvisibleCollider, false);
        GhostMesh = EditorPrefs.GetBool(Key_GhostMesh, false);
        Valuables = EditorPrefs.GetBool(Key_Valuables, false);
        Paths = EditorPrefs.GetBool(Key_Paths, false);
    }

    private static void SavePrefs()
    {
        EditorPrefs.SetBool(Key_InvisibleCollider, InvisibleCollider);
        EditorPrefs.SetBool(Key_GhostMesh, GhostMesh);
        EditorPrefs.SetBool(Key_Valuables, Valuables);
        EditorPrefs.SetBool(Key_Paths, Paths);
    }

    private static bool _invisibleCollider;
    private static bool _ghostMesh;
    private static bool _valuables;
    private static bool _paths;

    // Stores the GhostMesh state before entering Play Mode
    private static bool _ghostMeshBeforePlayMode;

    public static bool InvisibleCollider
    {
        get => _invisibleCollider;
        set { _invisibleCollider = value; SavePrefs(); SceneView.RepaintAll(); }
    }

    public static bool GhostMesh
    {
        get => _ghostMesh;
        set { _ghostMesh = value; SavePrefs(); SceneView.RepaintAll(); }
    }

    public static bool Valuables
    {
        get => _valuables;
        set { _valuables = value; SavePrefs(); SceneView.RepaintAll(); }
    }

    public static bool Paths
    {
        get => _paths;
        set { _paths = value; SavePrefs(); SceneView.RepaintAll(); }
    }

    // --- Play Mode Handling for GhostMesh ---
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.EnteredPlayMode:
                // Save previous state and disable Ghost visualizer
                _ghostMeshBeforePlayMode = GhostMesh;
                GhostMesh = false;
                break;

            case PlayModeStateChange.ExitingPlayMode:
                // Restore previous state
                GhostMesh = _ghostMeshBeforePlayMode;
                break;
        }
    }

    // --- Menu Items (Alt shortcuts) ---

    [MenuItem("Gizmos/Toggle Invisible Collider Visualizer &g")] // Alt+G
    private static void ToggleInvisibleCollider() => InvisibleCollider = !InvisibleCollider;

    [MenuItem("Gizmos/Toggle Invisible Collider Visualizer &g", true)]
    private static bool ValidateInvisibleCollider()
    {
        Menu.SetChecked("Gizmos/Toggle Invisible Collider Visualizer", InvisibleCollider);
        return true;
    }

    [MenuItem("Gizmos/Toggle Ghost Mesh Visualizer &j")] // Alt+J
    private static void ToggleGhostMesh() => GhostMesh = !GhostMesh;

    [MenuItem("Gizmos/Toggle Ghost Mesh Visualizer &j", true)]
    private static bool ValidateGhostMesh()
    {
        Menu.SetChecked("Gizmos/Toggle Ghost Mesh Visualizer", GhostMesh);
        return true;
    }

    [MenuItem("Gizmos/Toggle Valuables Visualizer &v")] // Alt+V
    private static void ToggleValuables() => Valuables = !Valuables;

    [MenuItem("Gizmos/Toggle Valuables Visualizer &v", true)]
    private static bool ValidateValuables()
    {
        Menu.SetChecked("Gizmos/Toggle Valuables Visualizer", Valuables);
        return true;
    }

    [MenuItem("Gizmos/Toggle Paths Visualizer &k")] // Alt+K
    private static void TogglePaths() => Paths = !Paths;

    [MenuItem("Gizmos/Toggle Paths Visualizer &k", true)]
    private static bool ValidatePaths()
    {
        Menu.SetChecked("Gizmos/Toggle Paths Visualizer", Paths);
        return true;
    }
}
