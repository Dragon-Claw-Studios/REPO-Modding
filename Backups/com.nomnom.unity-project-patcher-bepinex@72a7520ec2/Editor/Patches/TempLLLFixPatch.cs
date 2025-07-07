#if ENABLE_BEPINEX
using System.Reflection;
using BepInEx.Bootstrap;
using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace Nomnom.BepInEx.Editor.Patches {
    [InitializeOnLoad]
    internal static class TempLLLFix {
        static TempLLLFix() {
            Debug.Log("TempLLLFixPatch: Initializing");
            if (EditorApplication.isPlayingOrWillChangePlaymode) {
                var harmony = new Harmony("TempLLLFixPatch");
                harmony.PatchAll(typeof(TempLLLFixPatch));
                Debug.Log("TempLLLFixPatch: Patched!");
            }
        }
    }
    
    [HarmonyPatch(typeof(Object), nameof(Object.hideFlags), MethodType.Setter)]
    internal sealed class TempLLLFixPatch {
        [HarmonyPrefix]
        private static bool Prefix(Object __instance) {
            if (__instance && __instance.name == "LethalLevelLoader AssetBundleLoader") {
                Debug.LogWarning("TempLLLFixPatch: Temporarily disabling the disabling of \"LethalLevelLoader AssetBundleLoader\"");
                return false;
            }

            return true;
        }
    }
}
#endif