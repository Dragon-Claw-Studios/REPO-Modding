using UnityEngine;
#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
#endif

public static class PathsVisualizerBridge
{
#if UNITY_EDITOR
    private static bool _initialized = false;
    private static PropertyInfo _propInfo;
    private static Func<bool> _getter;

    private static void EnsureInit()
    {
        if (_initialized) return;
        _initialized = true;

        Type foundType = Type.GetType("ColliderVisualizerToggles, Assembly-CSharp-Editor");
        if (foundType != null)
            _propInfo = foundType.GetProperty("Paths", BindingFlags.Public | BindingFlags.Static);

        if (_propInfo != null)
            _getter = () => (bool)_propInfo.GetValue(null);
        else
            _getter = () => EditorPrefs.GetBool("PathsVisualizerToggle", false);
    }

    public static bool Show
    {
        get { EnsureInit(); return _getter(); }
    }
#else
    public static bool Show => false;
#endif
}
