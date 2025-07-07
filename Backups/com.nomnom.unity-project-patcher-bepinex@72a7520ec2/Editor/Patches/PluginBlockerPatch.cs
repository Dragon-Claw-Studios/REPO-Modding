#if ENABLE_BEPINEX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Nomnom.UnityProjectPatcher.Editor;
using UnityEngine;

namespace Nomnom.BepInEx.Editor.Patches {
    [InjectPatch(PatchLifetime.Always)]
    internal static class PluginBlockerPatch {
        public static IEnumerable<MethodBase> TargetMethods() {
            var allSettings = BepInExPreloader.GetAllPluginBlockerUserSettings();
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetValidTypes())
                .Where(x => typeof(BaseUnityPlugin).IsAssignableFrom(x))
                .Where(x => allSettings.Any(y => y.PluginFullTypeNames.Contains(x.FullName)))
                .Select(x => AccessTools.Method(x, "Awake"))
                .Where(x => x != null)
                .Append(AccessTools.Method(typeof(PluginBlockerPatch), nameof(PluginBlockerPatch.Awake)));
        }

        public static bool Prefix(object __instance) {
            Debug.Log($"Blocking plugin: {__instance.GetType().FullName}");
            return false;
        }
        
        private static void Awake() { }
    }
}
#endif