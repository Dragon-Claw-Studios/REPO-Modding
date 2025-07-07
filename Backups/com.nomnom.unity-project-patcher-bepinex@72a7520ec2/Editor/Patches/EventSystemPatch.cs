#if ENABLE_BEPINEX
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nomnom.BepInEx.Editor.Patches {
    [HarmonyPatch(typeof(EventSystem))]
    [InjectPatch]
    internal static class EventSystemPatch {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void RemoveDuplicateEventSystems(ref List<EventSystem> ___m_EventSystems) {
            if (!Application.isPlaying) {
                return;
            }
    
            if (___m_EventSystems.Count == 1) return;
            for (var i = 1; i < ___m_EventSystems.Count; i++) {
                var eventSystem = ___m_EventSystems[i];
                eventSystem.enabled = false;
                
                Debug.LogWarning($"Removed duplicate EventSystem: at {i} where length is {___m_EventSystems.Count}");
                ___m_EventSystems.Remove(eventSystem);
            }
        }
    }
}
#endif